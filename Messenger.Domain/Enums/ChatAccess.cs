namespace Messenger.Domain.Enums;
[Flags]
public enum ChatAccess
{
    ReadMessages = 1,
    ReactMessages = 2,
    WriteMessages = 4,
    DeleteMessage = 8,
    AddParticipant = 16,
    RemoveParticipant = 32,
    RenameChat = 64,
    DeleteChat = 128,
    AssignAdmin = 256,
    ManageAccess = 512,
}