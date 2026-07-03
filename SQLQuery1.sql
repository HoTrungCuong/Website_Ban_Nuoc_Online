INSERT INTO Loais (TenLoai)
VALUES 
(N'Cà phê'),
(N'Trà sữa'),
(N'Nước ép'),
(N'Sinh tố'),
(N'Bánh ngọt');
INSERT INTO Mons (TenMon, GiaBan, MoTa, HinhAnh, MaLoai)
VALUES
(N'Bạc xỉu', 25000, N'Bạc xỉu thơm ngon', N'bacxiu.jpg', 1),
(N'Cà phê đen', 22000, N'Cà phê đen đậm vị', N'capheden.jpg', 1),
(N'Cà phê dừa', 30000, N'Cà phê dừa béo thơm', N'caphedua.jpg', 1),
(N'Cà phê kem', 32000, N'Cà phê kem mát lạnh', N'caphekem.jpg', 1),
(N'Cà phê muối', 30000, N'Cà phê muối thơm béo', N'caphemuoi.jpg', 1),
(N'Cà phê sữa', 25000, N'Cà phê sữa truyền thống', N'caphesua.jpg', 1),
(N'Cà phê sữa đá', 25000, N'Cà phê sữa đá thơm ngon', N'caphesuada.jpg', 1),
(N'Cà phê trứng', 35000, N'Cà phê trứng béo ngậy', N'caphetrung.jpg', 1),

(N'Trà sữa đào', 30000, N'Trà sữa vị đào', N'trasuadao.jpg', 2),
(N'Trà sữa matcha', 32000, N'Trà sữa matcha thơm béo', N'trasuamatcha.jpg', 2),
(N'Trà sữa ô long', 32000, N'Trà sữa ô long đậm vị', N'trasuaolong.jpg', 2),
(N'Trà sữa socola', 32000, N'Trà sữa socola', N'trasuasocola.png', 2),
(N'Trà sữa thái xanh', 30000, N'Trà sữa thái xanh', N'trasuathaixanh.jpg', 2),
(N'Trà sữa trân châu', 30000, N'Trà sữa trân châu', N'trasuatranchau.jpg', 2),
(N'Trà sữa truyền thống', 28000, N'Trà sữa truyền thống', N'trasuatruyenthong.jpg', 2),

(N'Ép cam', 28000, N'Nước ép cam tươi', N'epcam.jpg', 3),
(N'Ép dâu', 30000, N'Nước ép dâu tươi', N'epdau.jpg', 3),
(N'Ép dứa', 28000, N'Nước ép dứa', N'epdua.jpg', 3),
(N'Ép lựu', 32000, N'Nước ép lựu', N'epluu.jpg', 3),
(N'Nước cam', 28000, N'Nước cam tươi', N'nuoccam.jpg', 3),
(N'Nước chanh dây', 28000, N'Nước chanh dây', N'nuocchanhday.jpg', 3),
(N'Nước nha đam', 25000, N'Nước nha đam thanh mát', N'nuocnhadam.jpg', 3),

(N'Sinh tố bơ', 35000, N'Sinh tố bơ béo mịn', N'sinhtobo.jpg', 4),
(N'Sinh tố dâu', 35000, N'Sinh tố dâu tươi', N'sinhtodau.jpg', 4),
(N'Sinh tố mãng cầu', 35000, N'Sinh tố mãng cầu', N'sinhtomangcau.jpg', 4),

(N'Bánh chocolate', 35000, N'Bánh chocolate ngọt thơm', N'banhchocolate.jpg', 5),
(N'Bánh croissant', 30000, N'Bánh croissant giòn thơm', N'banhcroissant.jpg', 5),
(N'Bánh kem dâu', 40000, N'Bánh kem dâu', N'banhkemdau.jpg', 5),
(N'Bánh muffin', 30000, N'Bánh muffin mềm thơm', N'banhmuffin.jpg', 5),
(N'Cupcake', 28000, N'Cupcake nhỏ xinh', N'cupcake.jpg', 5),
(N'Mousse', 35000, N'Bánh mousse mềm mịn', N'mousse.jpg', 5),
(N'Pudding', 25000, N'Pudding béo mịn', N'pudding.jpg', 5),
(N'Tiramisu', 40000, N'Bánh tiramisu', N'tiramisu.jpg', 5),
(N'Su kem', 25000, N'Bánh su kem', N'sukem.jpg', 5);

-- 3. KhachHangs
INSERT INTO KhachHangs
(HoTen, Email, MatKhau, GioiTinh, NgaySinh, NgayDangKy, SDT, DiaChi, SoDu, UserName, Avatar)
VALUES
(N'Nguyễn Văn A', N'a@gmail.com', N'123456', N'Nam', '2003-01-01', GETDATE(), '0901111111', N'TP.HCM', 100000, N'vana', N'avatar1.jpg'),
(N'Trần Thị B', N'b@gmail.com', N'123456', N'Nữ', '2004-05-10', GETDATE(), '0902222222', N'Hà Nội', 50000, N'thib', N'avatar2.jpg');

-- 4. Admins
INSERT INTO Admins
(Username, FullName, Password, Email, Phone, CCCD, Gender, Address, CreatedAt)
VALUES
(N'admin', N'Quản trị viên', N'123456', N'admin@gmail.com', '0909999999', '012345678901', N'Nam', N'TP.HCM', GETDATE());

-- 5. NhanViens
INSERT INTO NhanViens
(HoTen, GioiTinh, NgaySinh)
VALUES
(N'Nhân viên 1', N'Nam', '2002-02-02'),
(N'Nhân viên 2', N'Nữ', '2003-03-03');