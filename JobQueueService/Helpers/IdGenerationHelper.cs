using System.Security.Cryptography;
using System.Text;

namespace JobQueueService.Helpers;

public static class IdGenerationHelper
{
    private static readonly MD5 _hashAlgorithm = MD5.Create();
    public static Guid GenerateGuid(string value)
    {
        byte[] stringBytes = Encoding.UTF8.GetBytes(value);
        byte[] generatedBytes = _hashAlgorithm.ComputeHash(stringBytes);
        return new Guid(generatedBytes);
    }
}