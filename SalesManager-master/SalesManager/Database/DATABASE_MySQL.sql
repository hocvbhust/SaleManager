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

insert into nhanvien values ('NVQL', 'Van Hoc', '1991-12-20 00:00:00', '031091003081', 'HP', '2022-02-13 00:00:00', '56', 'hocvb.hust@gmail.com');	
INSERT INTO `doanlttq_quanlyhanghoa`.`CUAHANG` (`MACH`, `TENCH`, `MACHUCH`, `NGTHANHLAP`) VALUES ('1', 'XiaoHaha', 'NVQL', '2022-02-12');

CREATE TABLE Goods_Type (
	Type_Id int not null auto_increment,
	Type_Name NVARCHAR(50),
	created_date datetime default now(),
    PRIMARY KEY (Type_Id)
	);


CREATE TABLE LOAIHANG (
	MAHANG int(7)  unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	TENHANG NVARCHAR(100),
	DVT NVARCHAR(10),
	Type_id int,
	CONSTRAINT PK_GOODSTYPE FOREIGN KEY (Type_id)
			REFERENCES Goods_Type(Type_Id));

CREATE TABLE NHAPHANG (
	MAHANG int(7) unsigned zerofill,
	MALO INT,
	NGNHAP DATETIME,
	HANSD DATETIME,
	SOLUONG INT,
	DONGIA decimal(15,2),
	MANV VARCHAR(15),
	CONSTRAINT PK_NHAPHANG PRIMARY KEY (MAHANG, MALO),
	CONSTRAINT FK_NHAPHANG_LOAIHANG FOREIGN KEY (MAHANG)
			REFERENCES LOAIHANG(MAHANG),
	CONSTRAINT FK_NHAPHANG_NHANVIEN FOREIGN KEY (MANV)
			REFERENCES NHANVIEN (MANV));




