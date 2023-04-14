public interface IMLPhysicsObject {
    public ref PhysicsObject GetPhysicsObject();

    public MLPhysics.Rect[] GetHurtBoxes();

    public MLPhysics.Rect[] GetHitboxes();
}