namespace Shift.Constant
{
    public enum PermissionOperation
    {
        Read,         // R = Select = View
        Write,        // W = Update = Edit
        Delete,       // D = Delete = Remove
        Configure,    // F = Full Control
        
        // New operations
        Create,       // C = Insert = Add
        Administrate, // A
        Execute       // X
    }
}