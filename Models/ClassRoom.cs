/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

using LabW06.Exceptions;

namespace LabW06.Models;

public sealed class ClassRoom
{
    private readonly List<Device> devices = new();
    public string RoomId { get; }
    public string Name { get; }
    public int Capacity { get; }
    public IReadOnlyList<Device> Devices { get; }

    public ClassRoom(string roomId, string name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new ArgumentException("Mã phòng không được rỗng.", nameof(roomId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên phòng không được rỗng.", nameof(name));
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Sức chứa phải lớn hơn 0.");
        RoomId = roomId.Trim();
        Name = name.Trim();
        Capacity = capacity;
        Devices = devices.AsReadOnly();
    }

    public void AddDevice(Device device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (FindDevice(device.DeviceId) != null)
            throw new DuplicateDeviceException(device.DeviceId, "phòng học");
        devices.Add(device);
    }

    public bool RemoveDevice(string deviceId)
    {
        Device? device = FindDevice(deviceId);
        return device != null && devices.Remove(device);
    }

    public Device? FindDevice(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Mã thiết bị không được rỗng.", nameof(deviceId));
        return devices.Find(d => string.Equals(d.DeviceId, deviceId.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public decimal CalculateAnnualMaintenanceCost()
    {
        decimal total = 0;
        // Mỗi lớp thiết bị tự xác định chi phí bảo trì của mình.
        foreach (Device device in devices)
            total += device.CalculateAnnualMaintenanceCost();
        return total;
    }

    public List<Device> GetDevicesRequiringMaintenance()
    {
        int currentYear = DateTime.Today.Year;
        return devices.FindAll(d => d.Status == DeviceStatus.UnderMaintenance
                                    || currentYear - d.YearOfUse > 5);
    }
}
