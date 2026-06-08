using Assets.Scripts;

public interface IMovable
{
    void PickUp();
    void Drop();
    bool IsMoving { get; }

    public LaboratoryObject GetLaboratoryObject();
}
