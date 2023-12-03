public interface IMLCharacterPhysicsObject : IMLPhysicsObject {
    public MLCharacter GetCharacter();

    bool CanUseHitboxes();

    void UseHitboxesOn(IMLCharacterPhysicsObject character, int frameNumber);
}