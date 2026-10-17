/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Interfaces;

public interface IBorrower
{
    string BorrowerId { get; }
    string FullName { get; }
    int GetMaximumDevices();
}
