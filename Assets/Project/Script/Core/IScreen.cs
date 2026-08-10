using Gazeus.DesafioMatch3.ScriptableObjects;

namespace Gazeus.DesafioMatch3.Core
{
    public interface IScreen
    {
        string Name { get; }
        ScreenDefinition Definition { get; }
        bool OpenOnRegister { get; }

        void SetVisible(bool visible);
        void SetSortingOrder(int order);
    }
}
