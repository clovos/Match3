using InputWrapper;
using Messaging;
using Storage;
using Zenject;

public class GameInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.Bind<IDatabase>().To<PlayerPrefsDatabase>().AsSingle().NonLazy();
		Container.Bind<IMessenger>().To<SimpleMessenger>().AsSingle().NonLazy();
		Container.Bind<IInputBlocker>().To<GenericInputBlocker>().AsSingle().NonLazy();
	}
}
