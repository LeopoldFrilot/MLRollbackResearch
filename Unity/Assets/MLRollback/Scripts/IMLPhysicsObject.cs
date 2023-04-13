public interface IMLPhysicsObject {
    public ref PhysicsObject GetPhysicsObject();

    public MLPhysics.Rect[] GetColliders();

    public MLPhysics.Rect[] GetTriggers();
}