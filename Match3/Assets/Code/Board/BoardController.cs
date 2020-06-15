using System;
using Messaging;
using Level;
using Tile.DataTypes;
using InputWrapper;
using Messaging.Messages;
using Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Board
{
	public class BoardController
	{
		private readonly BoardModel _model;
		private readonly BoardView _view;
		
		private readonly BoardConfig _config;
		private readonly GameSettingsConfig _gameSettingsConfig;
		private readonly InputBlocker _inputBlocker;
		private readonly IMessenger _messenger;
		private readonly InputEventReporter _inputEventReporter;

		public BoardController(
			BoardModel model, 
			BoardView view, 
			BoardConfig config, 
			GameSettingsConfig gameSettingsConfig, 
			InputEventReporter inputEventReporter,
			InputBlocker inputBlocker,
			IMessenger messenger)
		{
			_model = model;
			_view = view;
			_config = config;
			_gameSettingsConfig = gameSettingsConfig;
			_inputBlocker = inputBlocker;
			_messenger = messenger;
			_inputEventReporter = inputEventReporter;
			
			var steps = CreateBoard(0);
			UpdateBoard(steps);
			_inputEventReporter.OnSwipe += OnSwipe;
		}

		private int CreateBoard(int startingSteps)
		{
			_model.Tiles = new TileModel[_config.boardHeight, _config.boardWidth];
			
			//Formula for creating board without matches
			for (int y = 0; y < _config.boardHeight; y++)
			{
				for (int x = 0; x < _config.boardWidth; x++)
				{
					TileConfig randomTileConfig;
					do
					{
						randomTileConfig = _config.tiles[Random.Range(0, _config.tiles.Length)];
					}
					while ((x >= 2 &&
					        _model.Tiles[y, (x - 1)].Type == randomTileConfig.type &&
					        _model.Tiles[y, (x - 2)].Type == randomTileConfig.type)
					       ||
					       (y >= 2 &&
					        _model.Tiles[(y - 1), x].Type == randomTileConfig.type &&
					        _model.Tiles[(y - 2), x].Type == randomTileConfig.type));

					var offsetPositionToStartWith = -_config.boardWidth;
					_model.Tiles[y, x] = CreateTile(randomTileConfig, x, y, x, offsetPositionToStartWith);
					var whatStepToFallOn = startingSteps + _config.boardHeight - y - 1;
					_model.Tiles[y, x].SetPosition(CalculateTileWorldPosition(x, y), whatStepToFallOn);
				}
			}
			return startingSteps + _config.boardHeight;
		}

		private void UpdateBoard(int startingStep)
		{
			var steps = startingStep;
			do
			{
				var iterationStepStartAmount = steps;
				
				MarkMatchedTiles(steps);
				RegisterMatchPoints(steps);
				RepositionTiles(steps);
				SpawnNewTiles(steps);
				
				if (!EvaluateBoardForMatches() && !EvaluateBoardForAnyPotentialMatches())
				{
					Debug.Log("No matches left!");
					steps++;
						
					//No matches left, reset board
					MarkAllTilesForReplacement(steps++);
					steps = CreateBoard(steps++);
				}
				steps++;

				var iteratedSteps = steps - iterationStepStartAmount;
				_inputBlocker.AddBlockTime(iteratedSteps * _gameSettingsConfig.stepTime);
			} while ((EvaluateBoardForMatches() || !EvaluateBoardForAnyPotentialMatches()) && steps < 100);
			
			if(steps >= 100) throw new OverflowException("Board iterations was turned off because of a infinite loop!");
			
			_messenger.Publish(new BoardIdleMessage{Step = steps});
		}

		private void MarkAllTilesForReplacement(int steps)
		{
			for (var x = 0; x < _config.boardWidth; x++)
			{
				var tileOffset = -1;
				for (var y = _config.boardHeight - 1; y >= 0; y--)
				{
					_model.Tiles[y, x].SetState(TileState.Replace, steps);
				}
			}
		}

		private TileModel CreateRandomTile(int x, int y, int offsetX, int offsetY)
		{
			var randomTile = _config.tiles[Random.Range(0, _config.tiles.Length)];
			return CreateTile(randomTile, x, y, offsetX, offsetY);
		}
		
		private TileModel CreateTile(TileConfig config, int x, int y, int offsetX, int offsetY)
		{
			var tileFactory = new TileFactory();
			var tilePosition = CalculateTileWorldPosition(offsetX, offsetY);
			return tileFactory.Load(config, tilePosition, _view.transform, _gameSettingsConfig, x , y);
		}

		private void SpawnNewTiles(int steps)
		{
			for (var x = 0; x < _config.boardWidth; x++)
			{
				var tileYOffset = -1;
				for (var y = _config.boardHeight - 1; y >= 0; y--)
				{
					var tile = _model.Tiles[y, x];
					if (tile.State != TileState.Match && tile.State != TileState.Replace) continue;

					_model.Tiles[y, x] = CreateRandomTile(x, y, x, tileYOffset);
					_model.Tiles[y, x].SetPosition(CalculateTileWorldPosition(x, y), steps);
					tileYOffset--;
				}
			}
		}

		private void RepositionTiles(int steps)
		{
			for (var x = 0; x < _config.boardWidth; x++)
			{
				for (var y = _config.boardHeight - 1; y >= 0; y--)
				{
					var tile = _model.Tiles[y, x];
					if (tile.State != TileState.Match) continue;
					
					var closestUnmarkedTileYPos =  GetClosestUnmarkedTileYPos(x, y);
					if (closestUnmarkedTileYPos == y) continue;
					
					_model.Tiles[y, x] = _model.Tiles[closestUnmarkedTileYPos, x];
					_model.Tiles[y, x].SetPosition(CalculateTileWorldPosition(x, y), steps);
					
					_model.Tiles[closestUnmarkedTileYPos, x] = tile;
				}
			}
		}

		private int GetClosestUnmarkedTileYPos(int fromX, int fromY)
		{
			for (var y = fromY - 1; y >= 0; y--)
			{
				if (_model.Tiles[y, fromX].State != TileState.Match) return y;
			}
			return fromY;
		}

		private void MarkMatchedTiles(int steps)
		{
			for (var y = 0; y < _config.boardHeight; y++)
			{
				for (var x = 0; x < _config.boardWidth; x++)
				{
					var tileType = _model.Tiles[y, x].Type;
					if (!IsMatching(tileType, x, y)) continue;
					
					_model.Tiles[y, x].SetState(TileState.Match, steps);
					MarkNeighbors(tileType, x, y, steps);
				}
			}
		}
		
		private void RegisterMatchPoints(int steps)
		{
			var score = 0;
			for (var y = 0; y < _config.boardHeight; y++)
			{
				for (var x = 0; x < _config.boardWidth; x++)
				{
					if (_model.Tiles[y, x].State == TileState.Match)
					{
						score += _gameSettingsConfig.matchScore;
					}
				}
			}

			_messenger.Publish(new ScoreUpdateMessage{Score = score, Step = steps});
		}
		
		private bool EvaluateBoardForMatches()
		{
			var matchesWasFound = false;
			for (var y = 0; y < _config.boardHeight; y++)
			{
				for (var x = 0; x < _config.boardWidth; x++)
				{
					var tileType = _model.Tiles[y, x].Type;
					if (!IsMatching(tileType, x, y)) continue;
					matchesWasFound = true;
				}
			}
			return matchesWasFound;
		}
		
		private bool EvaluateBoardForAnyPotentialMatches()
		{
			for (var y = 0; y < _config.boardHeight; y++)
			{
				for (var x = 0; x < _config.boardWidth; x++)
				{
					var type = _model.Tiles[y, x].Type;
					if (IsNeighborAMatchWithMyType(type, x, y))
						return true;
				}
			}
			return false;
		}

		private bool IsNeighborAMatchWithMyType(TileType type, int originTileX, int originTileY)
		{
			var x = originTileX + 1;
			var y = originTileY;
			//Right
			if (IsNeighborSameType(type, x + 1, y) && IsNeighborSameType(type, x + 2, y)) return true;
			if (IsNeighborSameType(type, x, y + 1) && IsNeighborSameType(type, x, y + 2)) return true;
			if (IsNeighborSameType(type, x, y - 1) && IsNeighborSameType(type, x, y - 2)) return true;
			if (IsNeighborSameType(type, x, y + 1) && IsNeighborSameType(type, x, y - 1)) return true;
			
			x = originTileX - 1;
			y = originTileY;
			//Left
			if (IsNeighborSameType(type, x - 1, y) && IsNeighborSameType(type, x - 2, y)) return true;
			if (IsNeighborSameType(type, x, y + 1) && IsNeighborSameType(type, x, y + 2)) return true;
			if (IsNeighborSameType(type, x, y - 1) && IsNeighborSameType(type, x, y - 2)) return true;
			if (IsNeighborSameType(type, x, y + 1) && IsNeighborSameType(type, x, y - 1)) return true;

			x = originTileX;
			y = originTileY + 1;
			//Up
			if (IsNeighborSameType(type, x, y + 1) && IsNeighborSameType(type, x, y + 2)) return true;
			if (IsNeighborSameType(type, x + 1, y) && IsNeighborSameType(type, x + 2, y)) return true;
			if (IsNeighborSameType(type, x - 1, y) && IsNeighborSameType(type, x - 2, y)) return true;
			if (IsNeighborSameType(type, x - 1, y) && IsNeighborSameType(type, x + 1, y)) return true;

			x = originTileX;
			y = originTileY - 1;
			//Down
			if (IsNeighborSameType(type, x, y - 1) && IsNeighborSameType(type, x, y - 2)) return true;
			if (IsNeighborSameType(type, x + 1, y) && IsNeighborSameType(type, x + 2, y)) return true;
			if (IsNeighborSameType(type, x - 1, y) && IsNeighborSameType(type, x - 2, y)) return true;
			if (IsNeighborSameType(type, x - 1, y) && IsNeighborSameType(type, x + 1, y)) return true;

			return false;
		}

		private void MarkNeighbors(TileType type, int x, int y, int step)
		{
			if (IsNeighborSameType(type, x + 1, y) && IsNeighborSameType(type, x - 1, y))
			{
				_model.Tiles[y, x + 1].SetState(TileState.Match, step);
				_model.Tiles[y, x - 1].SetState(TileState.Match, step);
			}

			if (IsNeighborSameType(type, x, y + 1) && IsNeighborSameType(type, x, y - 1))
			{
				_model.Tiles[y + 1, x].SetState(TileState.Match, step);
				_model.Tiles[y - 1, x].SetState(TileState.Match, step);
			}
		}

		private bool IsMatching(TileType type, int x, int y)
		{
			return IsNeighborSameType(type, x + 1, y) && IsNeighborSameType(type, x - 1, y) ||
			    IsNeighborSameType(type, x, y + 1) && IsNeighborSameType(type, x, y - 1);
		}

		private bool IsNeighborSameType(TileType type, int x, int y)
		{
			if (IsPositionOutOfBounds(x, y)) return false;
			return _model.Tiles[y, x].Type == type;
		}

		private bool IsPositionOutOfBounds(int x, int y)
		{
			return x < 0 || x >= _config.boardWidth || y < 0 || y >= _config.boardHeight;
		}
		
		private void OnSwipe(TileWorldPosition startPosition, float angle)
		{
			var startTileIndex = CalculateWorldTilePosition(startPosition);
			if (IsPositionOutOfBounds(startTileIndex.X, startTileIndex.Y)) return;

			var endTileIndex = GetNeighborInAngleDirection(angle, startTileIndex);
			if (IsPositionOutOfBounds(endTileIndex.X, endTileIndex.Y)) return;
			
			//Swap tiles
			var startTile = _model.Tiles[startTileIndex.Y, startTileIndex.X];
			var endTile = _model.Tiles[endTileIndex.Y, endTileIndex.X];
			
			_model.Tiles[endTileIndex.Y, endTileIndex.X] = startTile;
			startTile.SetPosition(CalculateTileWorldPosition(endTileIndex.X, endTileIndex.Y), 0);
			
			_model.Tiles[startTileIndex.Y, startTileIndex.X] = endTile;
			endTile.SetPosition(CalculateTileWorldPosition(startTileIndex.X, startTileIndex.Y), 0);
			
			_inputBlocker.AddBlockTime(_gameSettingsConfig.stepTime);

			if (EvaluateBoardForMatches())
			{
				_messenger.Publish(new MoveUpdateMessage());
				UpdateBoard(1);
			}
			else
			{
				//Swap back
				_model.Tiles[endTileIndex.Y, endTileIndex.X] = endTile;
				startTile.SetPosition(CalculateTileWorldPosition(startTileIndex.X, startTileIndex.Y), 1);
			
				_model.Tiles[startTileIndex.Y, startTileIndex.X] = startTile;
				endTile.SetPosition(CalculateTileWorldPosition(endTileIndex.X, endTileIndex.Y), 1);
				
				_inputBlocker.AddBlockTime(_gameSettingsConfig.stepTime);
			}
		}

		private TilePosition GetNeighborInAngleDirection(float angle, TilePosition startTileIndex)
		{
			if (angle > 45 && angle <= 135) // Up
			{
				return new TilePosition {X = startTileIndex.X, Y = startTileIndex.Y - 1};
			}
			if (angle < -45 && angle >= -135) //Down
			{
				return new TilePosition {X = startTileIndex.X, Y = startTileIndex.Y + 1};
			}
			if (angle > -45 && angle <= 45) //Right
			{
				return new TilePosition {X = startTileIndex.X - 1, Y = startTileIndex.Y};
			}
			else // Left
			{
				return new TilePosition {X = startTileIndex.X + 1, Y = startTileIndex.Y};
			}
		}

		private TileWorldPosition CalculateTileWorldPosition(int x, int y)
		{
			var halfBoardWidth = (_config.boardWidth * _config.tileWidth) / 2f;
			var halfBoardHeight = (_config.boardHeight * _config.tileHeight) / 2f;
			var halfTileOffsetX = (_config.tileWidth / 2);
			var halfTileOffsetY = (_config.tileHeight / 2);
			
			var centeredPosX = halfBoardWidth - (x * _config.tileWidth);
			var centeredPosY = halfBoardHeight - (y * _config.tileHeight);
			return new TileWorldPosition{ X = centeredPosX - halfTileOffsetX, Y = centeredPosY - halfTileOffsetY };
		}
		
		private TilePosition CalculateWorldTilePosition(TileWorldPosition position)
		{
			var halfBoardWidth = (_config.boardWidth * _config.tileWidth) / 2f;
			var halfBoardHeight = (_config.boardHeight * _config.tileHeight) / 2f;
			var halfTileOffsetX = (_config.tileWidth / 2);
			var halfTileOffsetY = (_config.tileHeight / 2);

			var worldXPosWithoutOffsets = position.X + halfTileOffsetX + halfBoardWidth;
			var worldYPosWithoutOffsets = position.Y + halfTileOffsetY + halfBoardHeight;
			var tileXIndex = _config.boardWidth - (Mathf.RoundToInt(worldXPosWithoutOffsets / _config.tileWidth));
			var tileYIndex = _config.boardHeight - (Mathf.RoundToInt(worldYPosWithoutOffsets / _config.tileHeight));
			return new TilePosition {X = tileXIndex, Y = tileYIndex};
		}

		public void Destroy()
		{
			_inputEventReporter.OnSwipe -= OnSwipe;
		}
	}
}