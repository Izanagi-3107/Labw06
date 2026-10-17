/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Models;

public sealed class WirelessMicrophone : TeachingDevice
{
    public override int MaximumBorrowDays => 1;
    public override int MaximumBorrowPeriods => 3;
    public override decimal DailyLateFee => 20_000m;

    public WirelessMicrophone(string deviceId, string name, int yearOfUse, decimal purchasePrice)
        : base(deviceId, name, yearOfUse, purchasePrice)
    {
    }

    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.08m;
}
