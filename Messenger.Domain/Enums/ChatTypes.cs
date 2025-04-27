namespace Messenger.Domain.Enums;

public enum ChatTypes
{
    Chat,
    Dialog,
    Channel,
}

public enum ChatAccess
{
    ReadMessages = 1,
    ReactMessages = 2,
    WriteMessages = 4,
    DeleteMessage = 8,
    AddParticipant = 16,
    RemoveParticipant = 32,
}