/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Models;

public sealed class AirConditionerRemote : TeachingDevice
{
    public override int MaximumBorrowDays => 2;
    public override int MaximumBorrowPeriods => 5;
    public override decimal DailyLateFee => 10_000m;

    public AirConditionerRemote(string deviceId, string name, int yearOfUse, decimal purchasePrice)
        : base(deviceId, name, yearOfUse, purchasePrice)
    {
    }

    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.04m;
}
