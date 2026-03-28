using UI;

namespace Gameplay.Data
{
    public class ConfigurationData
    {
        public ConfigurationType Type { get; set; }
        public bool IsUnlocked { get; set; }
        public int Level { get; set; } = 1;
        public int AdditionalLevel { get; set; } = 1;
    }
}