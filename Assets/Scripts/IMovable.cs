public interface IMovable
{
    void PickUp();
    void Drop();
    bool IsMoving { get; }
}
