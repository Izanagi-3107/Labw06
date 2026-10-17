/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Models;

public abstract class Device
{
    public string DeviceId { get; }
    public string Name { get; }
    public int YearOfUse { get; }
    public decimal PurchasePrice { get; }
    public DeviceStatus Status { get; protected set; }

    protected Device(string deviceId, string name, int yearOfUse, decimal purchasePrice)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Mã thiết bị không được rỗng.", nameof(deviceId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên thiết bị không được rỗng.", nameof(name));
        if (yearOfUse < 1 || yearOfUse > DateTime.Today.Year)
            throw new ArgumentOutOfRangeException(nameof(yearOfUse), "Năm sử dụng không hợp lệ.");
        if (purchasePrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(purchasePrice), "Giá mua phải lớn hơn 0.");

        DeviceId = deviceId.Trim();
        Name = name.Trim();
        YearOfUse = yearOfUse;
        PurchasePrice = purchasePrice;
        Status = DeviceStatus.Available;
    }

    public void StartMaintenance()
    {
        if (Status != DeviceStatus.Available)
            throw new InvalidOperationException("Chỉ bảo trì thiết bị đang sẵn sàng.");
        Status = DeviceStatus.UnderMaintenance;
    }

    public void FinishMaintenance()
    {
        if (Status != DeviceStatus.UnderMaintenance)
            throw new InvalidOperationException("Thiết bị không ở trạng thái bảo trì.");
        Status = DeviceStatus.Available;
    }

    public abstract decimal CalculateAnnualMaintenanceCost();

    public override string ToString()
    {
        return $"{DeviceId} | {Name} | Năm: {YearOfUse} | Giá: {PurchasePrice:N0} VND | {Status}";
    }
}
