using Services.Currency;

namespace Gameplay.Entities.Items
{
    public interface ICatcheable
    {
        void Catch(ICatchHandler catchHandler);
    }

    public interface ICatchHandler
    {
        void EarnCurrency(CurrencyType type, int value);
        void EarnFuel(int value);
        void GetMagnet(float radius, float duration);
        void GetAccelerator(float percent, float duration);
    }
}