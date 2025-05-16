namespace Messenger.Domain.Enums;

[Flags]
public enum DocumentAccess
{
    ViewDocument = 1,
    DownloadDocument = 2,
    DeleteDocument = 4
}