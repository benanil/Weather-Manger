

namespace AnilTools
{
    public interface ITickable
    {
        void Tick();
        int InstanceId();
    }

    public enum UpdateType
    {
        fixedTime = 1, normal = 2 , SlowUpdate = 4, lateUpdate = 8
    }
    
    public enum MoveType
    {
        towards, lerp , Curve
    }
}