namespace DependencyInjection
{

    public class PhotoTakenSignal
    {
        public readonly byte[] PhotoData;
        public readonly string PhotoName;

        public PhotoTakenSignal(byte[] photoData, string photoName)
        {
            PhotoData = photoData;
            PhotoName = photoName;
        }
    }

    public class PhotoDataSignal
    {
        public readonly bool IsAnybodySmiling;
        public readonly string PhotoName;
        
        public PhotoDataSignal(bool isAnybodySmiling, string photoName)
        {
            IsAnybodySmiling = isAnybodySmiling;
            PhotoName = photoName;
        }
    }

    public class PhotoAnalyzedSignal : PhotoDataSignal
    {
        public PhotoAnalyzedSignal(bool isAnybodySmiling, string photoName) : base(isAnybodySmiling, photoName)
        {
        }
    }

    public class UpdateMiniatureSignal : PhotoDataSignal
    {
        public UpdateMiniatureSignal(bool isAnybodySmiling, string photoName) : base(isAnybodySmiling, photoName)
        {
        }
    }

    public class MiniaturesPreparedSignal
    {
    }

    public class PhotoSerializedSignal
    {
    }

    public class SwitchCameraSignal
    {
    }

    public class ClearAllDataSignal
    {
    }
}