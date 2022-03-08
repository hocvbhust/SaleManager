CREATE DATABASE DOANLTTQ_QUANLYHANGHOA;
USE DOANLTTQ_QUANLYHANGHOA;
			
CREATE TABLE NHANVIEN (
	MANV VARCHAR(15) PRIMARY KEY,
	HOTEN NVARCHAR(50),
	NGSINH DATETIME,
	CMND VARCHAR(15),
	DIACHI NVARCHAR(100),
	NGVAOLAM DATETIME,
	MATKHAU VARCHAR(200),
	GMAIL VARCHAR(200));
	
CREATE TABLE CUAHANG (
	MACH VARCHAR(20) PRIMARY KEY,
	TENCH NVARCHAR(50),
	MACHUCH VARCHAR(15),
	NGTHANHLAP DATETIME,
	CONSTRAINT FK_CUAHANG_NHANVIEN FOREIGN KEY (MACHUCH)
			REFERENCES NHANVIEN (MANV));

insert into nhanvien values ('NVQL', 'Van Hoc', '1991-12-20 00:00:00', '031091003081', 'HP', '2022-02-13 00:00:00', 'yVnuzPuX0E4=', 'hocvb.hust@gmail.com');	
INSERT INTO `doanlttq_quanlyhanghoa`.`CUAHANG` (`MACH`, `TENCH`, `MACHUCH`, `NGTHANHLAP`) VALUES ('1', 'XiaoHaha', 'NVQL', '2022-02-12');

CREATE TABLE Goods_Type (
	Type_Id int not null auto_increment,
	Type_Name NVARCHAR(50),
	created_date datetime default now(),
    PRIMARY KEY (Type_Id)
	);
insert into Goods_Type (Type_Name,created_date) values ('Quan Ao', CURDATE());
insert into Goods_Type (Type_Name,created_date) values ('My Pham', CURDATE());
insert into Goods_Type (Type_Name,created_date) values ('Phu Kien', CURDATE());
insert into Goods_Type (Type_Name,created_date) values ('Gau Bong', CURDATE());
insert into Goods_Type (Type_Name,created_date) values ('Trang suc', CURDATE());

CREATE TABLE LOAIHANG (
	MAHANG int(7)  unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	TENHANG NVARCHAR(100),
	DVT NVARCHAR(10),
	Type_id int,
	Type_Name NVARCHAR(50),
	CONSTRAINT PK_GOODSTYPE FOREIGN KEY (Type_id)
			REFERENCES Goods_Type(Type_Id));

CREATE TABLE NHAPHANG (
	id int not null auto_increment primary key, 
	MAHANG int(7) unsigned zerofill,
	NGNHAP DATETIME,
	SOLUONG INT,
	DONGIA decimal(15,2),
	MANV VARCHAR(15),
	CONSTRAINT FK_NHAPHANG_LOAIHANG FOREIGN KEY (MAHANG)
			REFERENCES LOAIHANG(MAHANG),
	CONSTRAINT FK_NHAPHANG_NHANVIEN FOREIGN KEY (MANV)
			REFERENCES NHANVIEN (MANV));
			



