# KivMusic-KP-
Информационная система "Магазин музыкальных инструментов" - KivMusic 

Скрипт базы данных

------------------------------------
--------Создание базы данных--------
------------------------------------
create database MusicShop owner postgres
--drop database

-----------------------------------
--------Таблицы и процедуры--------
-----------------------------------

create table Roles
(
	ID_Roles serial not null constraint PK_Roles primary key,
	Role_Name varchar(100) not null constraint UQ_Role_Name unique
);

create or replace procedure Roles_Insert(p_Role_Name varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from Roles
	where Role_Name = p_Role_Name;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		INSERT into Roles(Role_Name)
			values(p_Role_Name);
	end if;
	end;
$$;

create or replace procedure Roles_Update(p_ID_Roles int, p_Role_Name varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from Roles
	where Role_Name = p_Role_Name;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		UPDATE Roles set
		Role_Name = p_Role_Name
			where 
			ID_Roles = p_ID_Roles;
	end if;
	end;
$$;

create or replace procedure Roles_Delete(p_ID_Roles int)
language plpgsql
as $$
	DECLARE have_record int:= count(*) from Profile
	where Roles_ID = p_ID_Roles;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным id зайдействован';
	else 
		Delete from Roles 
			where ID_Roles = p_ID_Roles;
	end if;
	end;
$$;

create table Profile
(
	ID_Profile serial not null CONSTRAINT PK_Profile PRIMARY KEY,
	LastName varchar(100) not null,
	FirstName varchar(100) not null,
	MiddleName varchar(100) null,
	Profile_Login varchar(100) not null constraint UQ_Profile_Login unique,
	Profile_Password varchar(100) not null,
	Roles_ID int not null references Roles(ID_Roles)
);

create or replace procedure Profile_Insert(p_LastName varchar(100), p_FirstName varchar(100), p_MiddleName varchar(100), p_Profile_Login varchar(100), p_Profile_Password varchar(100), p_Roles_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Profile
where Profile_Login =p_Profile_Login;
begin
	if have_record>0 then
		raise exception 'Уже существует';
	else
		Insert into Profile(LastName, FirstName, MiddleName, Profile_Login, Profile_Password, Roles_ID)
		values(p_LastName, p_FirstName, p_MiddleName, p_Profile_Login, p_Profile_Password, p_Roles_ID);
	end if;
end;
$$;

create or replace procedure Profile_Update(p_ID_Profile int, p_LastName varchar(100), p_FirstName varchar(100), p_MiddleName varchar(100), p_Profile_Login varchar(100), p_Profile_Password varchar(100), p_Roles_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Profile
where Profile_Login = p_Profile_Login;
begin
	if have_record>0 then
		raise exception 'Уже существует';
	else
		UPDATE Profile set 
		User_First_Name = p_LastName,
		User_Patronymic = p_FirstName,
		User_Last_Name = p_MiddleName,
		Profile_Password = p_Profile_Password,
		Roles_ID = p_Roles_ID
		where 
			ID_Profile = p_ID_Profile;
	end if;
end;
$$;

create or replace procedure Users_Delete(p_ID_Profile int)
language plpgsql
as $$
Declare have_record int := count(*)from Profile_Card
where ID_Profile=p_ID_Profile;
begin
	if have_record>0 then
		raise exception 'Данный пользователь задействован';
	else
		Delete from Profile 
			where 
				ID_Profile = p_ID_Profile;
	end if;
end;
$$;

create table VacationType
(
	ID_VacationType serial not null constraint PK_VacationType primary key,
	TypeName varchar(100) not null constraint UQ_TypeName unique
);

create or replace procedure VacationType_Insert(p_TypeName varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from VacationType
	where TypeName = p_TypeName;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		INSERT into VacationType(TypeName)
			values(p_TypeName);
	end if;
	end;
$$;

create or replace procedure VacationType_Update(p_ID_VacationType int, p_TypeName varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from VacationType
	where TypeName = p_TypeName;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		UPDATE VacationType set
		TypeName = p_TypeName
			where 
			ID_VacationType = p_ID_VacationType;
	end if;
	end;
$$;

create or replace procedure VacationType_Delete(p_ID_VacationType int)
language plpgsql
as $$
	DECLARE have_record int:= count(*) from Vacation
	where VacationType_ID = p_ID_VacationType;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным id зайдействован';
	else 
		Delete from VacationType 
			where ID_VacationType = p_ID_VacationType;
	end if;
	end;
$$;

create table PayType
(
	ID_PayType serial not null constraint PK_PayType primary key,
	TypeName varchar(100) not null constraint UQ_PayTypeName unique
);

create or replace procedure PayType_Insert(p_TypeName varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from PayType
	where TypeName = p_TypeName;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		INSERT into PayType(TypeName)
			values(p_TypeName);
	end if;
	end;
$$;

create or replace procedure PayType_Update(p_ID_PayType int, p_TypeName varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from PayType
	where TypeName = p_TypeName;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		UPDATE VacationType set
		TypeName = p_TypeName
			where 
			ID_PayType = p_ID_PayType;
	end if;
	end;
$$;

create or replace procedure PayType_Delete(p_ID_PayType int)
language plpgsql
as $$
	DECLARE have_record int:= count(*) from Salary
	where PayType_ID = p_ID_PayType;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным id зайдействован';
	else 
		Delete from PayType 
			where ID_PayType = p_ID_PayType;
	end if;
	end;
$$;

create table SickType
(
	ID_SickType serial not null constraint PK_SickType primary key,
	TypeName varchar(100) not null constraint UQ_SickTypeName unique
);

create or replace procedure SickType_Insert(p_TypeName varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from SickType
	where TypeName = p_TypeName;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		INSERT into SickType(TypeName)
			values(p_TypeName);
	end if;
	end;
$$;

create or replace procedure SickType_Update(p_ID_SickType int, p_TypeName varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from SickType
	where TypeName = p_TypeName;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		UPDATE VacationType set
		TypeName = p_TypeName
			where 
			ID_SickType = p_ID_SickType;
	end if;
	end;
$$;

create or replace procedure SickType_Delete(p_ID_SickType int)
language plpgsql
as $$
	DECLARE have_record int:= count(*) from SickLeav
	where SickType_ID = p_ID_SickType;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным id зайдействован';
	else 
		Delete from SickType 
			where ID_SickType = p_ID_SickType;
	end if;
	end;
$$;

create table Salary
(
	ID_Salary serial not null constraint PK_Salary primary key,
	PayDate date not null,
	PaySum DECIMAL(38,2) not null,
	Employee_ID int not null references Profile(ID_Profile),
	Bookkeeper_ID int not null references Profile(ID_Profile),
	PayType_ID int not null references PayType(ID_PayType)
);

create table Vacation
(
	ID_Vacation serial not null constraint PK_Vacation primary key,
	StartVacationDate date not null,
	EndVacationDate date not null,
	Employee_ID int not null references Profile(ID_Profile),
	HRManager_ID int not null references Profile(ID_Profile),
	VacationType_ID int not null references VacationType(ID_VacationType)
);

create table SickLeav
(
	ID_SickLeav serial not null constraint PK_SickLeav primary key,
	StartDate date not null,
	EndDate date not null,
	Employee_ID int not null references Profile(ID_Profile),
	HRManager_ID int not null references Profile(ID_Profile),
	SickType_ID int not null references SickType(ID_SickType)
);

create table PaymentSystem
(
	ID_PaymentSystem serial not null constraint PK_PaymentSystem primary key,
	NameSystem varchar(100) not null constraint UQ_NameSystem unique
);

create or replace procedure Payment_system_Insert(p_Name_Payment_system varchar(30))
language plpgsql
as $$
Declare have_record int := count(*)from Payment_system
where Name_Payment_system=p_Name_Payment_system;
begin
	if have_record>0 then
		raise exception 'Такая платежная система существует';
	else
		insert into Payment_system(Name_Payment_system)
		values(p_Name_Payment_system);
	end if;
end;
$$;

create or replace procedure Payment_system_Update(p_ID_Payment_system int, p_Name_Payment_system varchar(30))
language plpgsql
as $$
Declare have_record int := count(*)from Payment_system
where Name_Payment_system=p_Name_Payment_system;
begin
	if have_record>0 then
		raise exception 'Такая платежная система существует';
	else
		UPDATE Shop Set
			Name_Payment_system=p_Name_Payment_system
			where 
				ID_Payment_system = p_ID_Payment_system;
	end if;
end;
$$;

create or replace procedure Payment_system_Delete(p_ID_Payment_system int)
language plpgsql
as $$
Declare have_record int := count(*)from Card
where Payment_system_ID=p_ID_Payment_system;
begin
	if have_record>0 then
		raise exception 'Данная платежная система используется';
	else
		delete from Payment_system 
			where ID_Payment_system = p_ID_Payment_system;
	end if;
end;
$$;

create table Bank
(
	ID_Bank serial not null constraint PK_Bank primary key,
	BankName varchar(100) not null constraint UQ_BankName unique
);

create or replace procedure Bank_Insert(p_Bank_Name varchar(30))
language plpgsql
as $$
Declare have_record int := count(*)from Bank
where Bank_Name=p_Bank_Name;
begin
	if have_record>0 then
		raise exception 'Такое название банка уже есть';
	else
		insert into Bank(Bank_Name)
		values(p_Bank_Name);
	end if;
end;
$$;

create or replace procedure Bank_Update(p_ID_Bank int, p_Bank_Name varchar(30))
language plpgsql
as $$
Declare have_record int := count(*)from Bank
where Bank_Name=p_Bank_Name;
begin
	if have_record>0 then
		raise exception 'Такое название банка уже есть';
	else
		UPDATE Bank Set
			Bank_Name=p_Bank_Name
			where 
				ID_Bank = p_ID_Bank;
	end if;
end;
$$;

create or replace procedure Bank_Delete(p_ID_Bank int)
language plpgsql
as $$
Declare have_record int := count(*)from Card
where Bank_ID=p_ID_Bank;
begin
	if have_record>0 then
		raise exception 'Данное банковское имя используется';
	else
		delete from Bank 
			where ID_Bank = p_ID_Bank;
	end if;
end;
$$;

create table TypeCard
(
	ID_TypeCard serial not null constraint PK_TypeCard primary key,
	TypeName varchar(100) not null constraint UQ_CardTypeName unique
);

create or replace procedure TypeCard_Insert(p_TypeName varchar(50))
language plpgsql
as $$
Declare have_record int := count(*)from TypeCard
where TypeName=p_TypeName;
begin
	if have_record>0 then
		raise exception 'Такой вид карты уже существует';
	else
		insert into TypeCard(TypeName)
			values(p_TypeName);
	end if;
end;
$$;

create or replace procedure TypeCard_Update(p_ID_TypeCard int, p_TypeName varchar(50))
language plpgsql
as $$
Declare have_record int := count(*)from TypeCard
where TypeName=p_TypeName;
begin
	if have_record>0 then
		raise exception 'Такой вид карты уже существует';
	else
		UPDATE TypeCard Set
			TypeName=p_TypeName
			where 
				ID_TypeCard = p_ID_TypeCard;
	end if;
end;
$$;

create or replace procedure TypeCard_Delete(p_ID_TypeCard int)
language plpgsql
as $$
Declare have_record int := count(*)from Card
where TypeCard_ID=p_ID_TypeCard;
begin
	if have_record>0 then
		raise exception 'Такой вид карты используется';
	else
		delete from TypeCard 
			where ID_TypeCard = p_ID_TypeCard;
	end if;
end;
$$;

create table Card
(
	ID_Card serial not null constraint PK_Card primary key,
	Card_Number varchar(30) not null constraint CH_Card_Number check (Card_Number similar to '([0-9]{4}-[0-9]{3}-[0-9]{3}-[0-9]{3}|[0-9]{4}-[0-9]{6}-[0-9]{5}|[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}|[0-9]{8}-[0-9]{10})'),
	Card_Holder varchar(30) not null,
	Card_Expiry_Date date not null,
	CVC_CCV varchar(3) not null constraint CH_CVC_CCV check (CVC_CCV similar to '[0-9][0-9][0-9]'),
	TypeCard_ID int not null references TypeCard(ID_TypeCard),
	Payment_System_ID int not null references PaymentSystem(ID_PaymentSystem),
	Bank_ID int not null references Bank(ID_Bank)
);

create or replace procedure Card_Insert(p_Card_Number varchar(30), p_Card_Holder varchar(30), p_Card_Expiry_Date date, p_CVC_CCV varchar(3), p_TypeCard_ID int, p_PaymentSystem_ID int, p_Bank_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Card
where Card_Number||''||Card_Holder||''||Card_Expiry_Date||''||CVC_CCV||''||TypeCard_ID||''||PaymentSystem_ID||''||Bank_ID =
p_Card_Number||''||p_Card_Holder||''||p_Card_Expiry_Date||''||p_CVC_CCV||''||p_TypeCard_ID||''||p_PaymentSystem_ID||''||p_Bank_ID;
begin
	if have_record>0 then
		raise exception 'Такая карта уже существует';
	else
		Insert into Card(Card_Number, Card_Holder, Card_Expiry_Date, CVC_CCV, TypeCard_ID, PaymentSystem_ID, Bank_ID)
		values(p_Card_Number, p_Card_Holder, p_Card_Expiry_Date, p_CVC_CCV, p_TypeCard_ID, p_PaymentSystem_ID, p_Bank_ID);
	end if;
end;
$$;

create or replace procedure Card_Update(p_ID_Card int, p_Card_Number varchar(30), p_Card_Holder varchar(30), p_Card_Expiry_Date date, p_CVC_CCV varchar(3), p_TypeCard_ID int, p_PaymentSystem_ID int, Bank_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Card
where Card_Number||''||Card_Holder||''||Card_Expiry_Date||''||CVC_CCV||''||TypeCard_ID||''||PaymentSystem_ID||''||Bank_ID =
p_Card_Number||''||p_Card_Holder||''||p_Card_Expiry_Date||''||p_CVC_CCV||''||p_TypeCard_ID||''||p_PaymentSystem_ID||''||p_Bank_ID;
begin
	if have_record>0 then
		raise exception 'Такая карта уже существует';
	else
		UPDATE Card set
		Card_Number = p_Card_Number,
		Card_Holder = p_Card_Holder,
		Card_Expiry_Date = p_Card_Expiry_Date,
		CVC_CCV = p_CVC_CCV,
		TypeCard_ID = p_TypeCard_ID,
		PaymentSystem_ID = p_PaymentSystem_ID,
		Bank_ID = p_Bank_ID
		where ID_Card = p_ID_Card;
	end if;
end;
$$;

create or replace procedure Card_Delete(p_ID_Card int)
language plpgsql
as $$
Declare have_record int := count(*)from Profile_Card
where ID_Card=p_ID_Card;
begin
	if have_record>0 then
		raise exception 'Нельзя удалить, так как она зайдествована';
	else
		Delete from Card
		where 
		ID_Card=p_ID_Card;
	end if;
end;
$$;

create table Shop
(
	ID_Shop serial not null constraint PK_Shop primary key,
	ShopName varchar(100) not null,
	ShopAddress varchar(100) not null
);

create or replace procedure Shop_Insert(p_ShopName varchar(30), p_ShopAddress varchar)
language plpgsql
as $$
Declare have_record int := count(*)from Shop
where ShopName=p_ShopName and ShopAddress=p_ShopAddress;
begin
	if have_record>0 then
		raise exception 'Такой магазин уже существует';
	else
		insert into Shop(ShopName, ShopAddress)
		values(p_ShopName, p_ShopAddress);
	end if;
end;
$$;

create or replace procedure Shop_Update(p_ID_Shop int, p_ShopName varchar(30), p_ShopAddress varchar)
language plpgsql
as $$
Declare have_record int := count(*)from Shop
where ShopName=p_ShopName and ShopAddress=p_Shop_ddress;
begin
	if have_record>0 then
		raise exception 'Такой магазин уже существует';
	else
		UPDATE Shop Set
			ShopName = p_ShopName,
			ShopAddress = p_ShopAddress
			where 
				ID_Shop = p_ID_Shop;
	end if;
end;
$$;

create or replace procedure Shop_Delete(p_ID_Shop int)
language plpgsql
as $$
Declare have_record int := count(*)from ProductCheck
where Shop_ID=p_ID_Shop;
begin
	if have_record>0 then
		raise exception 'Данный магазин испольузется в чеке';
	else
		delete from Shop 
		where ID_Shop = p_ID_Shop;
	end if;
end;
$$;

create table Characteristicz
(
	ID_Characteristicz serial not null constraint PK_Characteristicz primary key,
	NameCharacteristicz varchar not null constraint UQ_NameCharacteristicz unique
);

create or replace procedure Characteristicz_Insert(p_Name_Characteristicz varchar)
language plpgsql
as $$
Declare have_record int := count(*)from Characteristicz
where NameCharacteristicz = p_Name_Characteristicz;
begin
	if have_record>0 then
		raise exception 'Такая характеристика уже существует';
	else
		insert into Characteristicz(NameCharacteristicz)
		values(p_Name_Characteristicz);
	end if;
end;
$$;

create or replace procedure Characteristicz_Update(p_ID_Characteristicz int, p_Name_Characteristicz varchar)
language plpgsql
as $$
Declare have_record int := count(*)from Characteristicz
where NameCharacteristicz=p_Name_Characteristicz;
begin
	if have_record>0 then
		raise exception 'Такая характеристика уже существует';
	else
		UPDATE Characteristicz Set
		NameCharacteristicz = p_Name_Characteristicz
			where 
			ID_Characteristicz = p_ID_Characteristicz;
	end if;
end;
$$;

create or replace procedure Characteristicz_Delete(p_ID_Characteristicz int)
language plpgsql
as $$
DECLARE have_record int := count(*) from ProductCharacteristicz
	where Characteristicz_ID = p_ID_Characteristicz;
begin
	if have_record>0 then 
		raise exception 'Данная характеристик используется';
	else
		delete from Characteristicz 
			where ID_Characteristicz=p_ID_Characteristicz;
	end if;
end;
$$;

create table Product_Type
(
	ID_ProductType serial not null constraint PK_ProductType primary key,
	TypeName varchar(100) not null constraint UQ_ProductTypeName unique
);

create table Product
(
	ID_Product serial not null constraint PK_Product primary key,
	ProductName varchar(30) not null constraint UQ_ProductName unique,
	ProductPrice decimal(38,2) not null constraint CH_ProductPrice check (ProductPrice>0.0),
	ProductType_ID int not null references Product_Type(ID_ProductType)
);

create or replace procedure Product_Insert(p_Product_Name varchar(30), p_ProductPrice decimal(38,2))
language plpgsql
as $$
Declare have_record int := count(*)from Product
where Product_Name = p_Product_Name;
begin
	if have_record>0 then
		raise exception 'Такой товар уже есть';
	else
		insert into Product(Product_Name, ProductPrice)
		values(p_Product_Name, p_ProductPrice);
	end if;
end;
$$;

create or replace procedure Product_Update(p_ID_Product int, p_Product_Name varchar(30), p_ProductPrice decimal(38,2))
language plpgsql
as $$
Declare have_record int := count(*)from Product
where Product_Name = p_Product_Name and Product_Price = p_Product_Price;
begin
	if have_record>0 then
		raise exception 'Такой товар уже есть';
	else
		UPDATE Product Set
			Product_Name = p_Product_Name,
			ProductPrice = p_ProductPrice
			where 
				ID_Product = p_ID_Product;
	end if;
end;
$$;

create or replace procedure Product_Delete(p_ID_Product int)
language plpgsql
as $$
DECLARE have_record int := count(*) from Product inner join Product_Characteristicz on Product_ID = ID_Product
inner join Consumer_Cart on Consumer_Cart.Product_ID = ID_Product
inner join Location_in_warehouse on Location_in_warehouse.Product_ID = ID_Product
	where Product_Characteristicz.Product_Id = p_ID_Product;
begin
	if have_record>0 then 
		raise exception 'Данная товар используется';
	else
		delete from Product 
			where ID_Product=p_ID_Product;
	end if;
end;
$$;

create table ProductCharacteristicz
(
	ID_ProductCharacteristicz serial not null constraint PK_ProductCharacteristicz primary key,
	Product_ID int not null references Product(ID_Product),
	Characteristicz_ID int not null references Characteristicz(ID_Characteristicz)
);

create or replace procedure Product_Characteristicz_Insert(p_Product_ID int, p_Characteristicz_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from ProductCharacteristicz
where Product_ID=p_Product_ID and Characteristicz_ID=p_Characteristicz_ID;
begin
	if have_record>0 then
		raise exception 'Такой товар с характеристикой существует';
	else
		insert into ProductCharacteristicz(Product_ID, Characteristicz_ID)
		values(p_Product_ID, p_Characteristicz_ID);
	end if;
end;
$$;

create or replace procedure Product_Characteristicz_Update(p_ID_Product_Characteristicz int, p_Product_ID int, p_Characteristicz_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from ProductCharacteristicz
where Product_ID=p_Product_ID and Characteristicz_ID=p_Characteristicz_ID;
begin
	if have_record>0 then
		raise exception 'Такой товар с характеристикой существует';
	else
		UPDATE ProductCharacteristicz Set
			Product_ID = p_Product_ID,
			Characteristicz_ID = p_Characteristicz_ID
			where 
				ID_Product_Characteristicz = p_ID_Product_Characteristicz;
	end if;
end;
$$;

create or replace procedure Product_Characteristicz_Delete(p_ID_Product_Characteristicz int)
language plpgsql
as $$
begin
	delete from ProductCharacteristicz 
		where ID_Product_Characteristicz = p_ID_Product_Characteristicz;
end;
$$;

create table Profile_Card
(
	ID_Profile_Card serial not null constraint PK_Profile_Card primary key,
	Card_ID int not null references Card(ID_Card), 
	Profile_ID int not null references Profile(ID_Profile)
);

create or replace procedure Profile_Card_Insert(p_Profile_ID varchar(30), p_Card_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Profile_Card
where Profile_ID||''||Card_ID = p_Profile_ID||''||p_Card_ID;
begin
	if have_record>0 then
		raise exception 'Уже существует';
	else
		Insert into Profile_Card(Profile_ID, Card_ID)
		values(p_Profile_ID, p_Card_ID);
	end if;
end;
$$;

create or replace procedure Profile_Card_Update(p_ID_Profile_Card int, p_Profile_ID varchar(30), p_Card_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Profile_Card
where Users_Login||''||Card_ID = p_Users_Login||''||p_Card_ID;
begin
	if have_record>0 then
		raise exception 'Уже существует';
	else
	Update Profile_Card set 
	Profile_ID = p_Profile_ID,
	Card_ID = p_Card_ID
	where ID_Profile_Card = p_ID_Profile_Card;
	end if;
end;
$$;

create or replace procedure Profile_Card_Delete(p_ID_Profile_Card int)
language plpgsql
as $$
Declare have_record int := count(*)from Product_Check
where p_ID_Profile_Card=p_ID_Profile_Card;
begin
	if have_record>0 then
		raise exception 'Нельзя удалить, так как она зайдествована';
	else
	Delete from Profile_Card
	where 
	ID_Profile_Card=p_ID_Profile_Card;
	end if;
end;
$$;


create table Product_Check
(
	ID_Product_Check serial not null constraint PK_Product_Check primary key,
	Check_Number varchar(20) not null constraint CH_Check_Number check(Check_Number similar to '[0-9]{20}'),
	Shift_Number varchar(20) not null constraint CH_Shift_Number check(Shift_Number similar to '[0-9]{20}'),
	Purchase_Date date not null constraint CH_Purchase_Date check(Purchase_Date :: date = now() :: date ),
	Total_Sum decimal(38,2) not null constraint CH_Total_Sum check(Total_Sum>0.0),
	Input_Sum decimal(38,2) not null constraint CH_Input_Sum check(Input_Sum>0.0),
	Shop_ID int not null references Shop(ID_Shop),
	Profile_Card_ID int not null references Profile_Card(ID_Profile_Card),
	Profile_ID int not null references Profile(ID_Profile)	
);

create or replace procedure Product_Check_Insert(p_Check_Number varchar(20), p_Shift_Number varchar(20), p_Purchase_Date date, p_Total_Sum decimal(38,2), p_Input_Sum decimal(38,2), p_Shop_ID int, p_Profile_ID int, p_Profile_Card_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Product_Check
where Check_Number||''||Shift_Number||''||Purchase_Date||''||Total_Sum||''||Input_Sum||''||Shop_ID||''||Profile_ID||''||Profile_Card_ID = 
p_Check_Number||''||p_Shift_Number||''||p_Purchase_Date||''||p_Total_Sum||''||p_Input_Sum||''||p_Shop_ID||''||p_Profile_ID||''||p_Profile_Card_ID;
begin
	if have_record>0 then
		raise exception 'Уже существует';
	else
	Insert into Product_Check(Check_Number, Shift_Number, Purchase_Date, Total_Sum, Input_Sum, Shop_ID, Profile_ID, Profile_Card_ID)
	values (p_Check_Number, p_Shift_Number, p_Purchase_Date, p_Total_Sum, p_Input_Sum, p_Shop_ID, p_Profile_ID, p_Profile_Card_ID);
	end if;
end;
$$;

create or replace procedure Product_Check_Update(p_ID_Product_Check int, p_Check_Number varchar(20), p_Shift_Number varchar(20), p_Purchase_Date date, p_Total_Sum decimal(38,2), p_Input_Sum decimal(38,2), p_Shop_ID int, p_Profile_ID int, p_Profile_Card_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Product_Check
where Check_Number||''||Shift_Number||''||Purchase_Date||''||Total_Sum||''||Input_Sum||''||Shop_ID||''||Profile_ID||''||Profile_Card_ID = 
p_Check_Number||''||p_Shift_Number||''||p_Purchase_Date||''||p_Total_Sum||''||p_Input_Sum||''||p_Shop_ID||''||p_Profile_ID||''||p_Profile_Card_ID;
begin
	if have_record>0 then
		raise exception 'Уже существует';
	else
	Update Product_Check set
	Check_Number = p_Check_Number,
	Shift_Number = p_Shift_Number,
	Purchase_Date = p_Purchase_Date,
	Total_Sum = p_Total_Sum,
	Input_Sum = p_Input_Sum,
	Shop_ID = p_Shop_ID,
	Profile_ID = p_Profile_ID,
	Profile_Card_ID = p_Profile_Card_ID
	where ID_Product_Check = p_ID_Product_Check;
	end if;
end;
$$;

create or replace procedure Product_Check_Delete(p_ID_Product_Check int)
language plpgsql
as $$
Declare have_record int := count(*)from Consumer_Cart
where ID_Product_Check = p_ID_Product_Check;
begin
	if have_record>0 then
		raise exception 'Нельзя удалить, так как он зайдествована';
	else
	Delete from Product_Check
	where 
	ID_Product_Check = p_ID_Product_Check;
	end if;
end;
$$;



create table Consumer_Cart
(
	ID_Consumer_Cart serial not null constraint PK_Consumer_Cart primary key,
	Quantity_Of_Product int not null constraint CH_Quantity_Of_Product check(Quantity_Of_Product>0),
	Product_ID int not null references Product(ID_Product),
	Product_Check_ID int not null references Product_Check(ID_Product_Check)
);

create or replace procedure Consumer_Cart_Insert(p_Quantity_Of_Product int, p_Product_ID int, p_Product_Check_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Consumer_Cart
where Quantity_Of_Product||''||Product_ID||''||Product_Check_ID = 
p_Quantity_Of_Product||''||p_Product_ID||''||p_Product_Check_ID;
begin
	if have_record>0 then
		raise exception 'Уже существует';
	else
	Insert into Consumer_Cart(Quantity_Of_Product, Product_ID, Product_Check_ID)
	values(p_Quantity_Of_Product, p_Product_ID, p_Product_Check_ID);
	end if;
end;
$$;

create or replace procedure Consumer_Cart_Update(p_ID_Consumer_Cart int, p_Quantity_Of_Product int, p_Product_ID int, p_Product_Check_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from Consumer_Cart
where Quantity_Of_Product||''||Product_ID||''||Product_Check_ID = 
p_Quantity_Of_Product||''||p_Product_ID||''||p_Product_Check_ID;
begin
	if have_record>0 then
		raise exception 'Уже существует';
	else
	Update Consumer_Cart set
	Quantity_Of_Product = p_Quantity_Of_Product,
	Product_ID = p_Product_ID, 
	Product_Check_ID = p_Product_Check_ID
	where ID_Consumer_Cart = p_ID_Consumer_Cart;
	end if;
end;
$$;

create or replace procedure Consumer_Cart_Delete(p_ID_Consumer_Cart int)
language plpgsql
as $$
begin
	Delete from Consumer_Cart
		where ID_Consumer_Cart = p_ID_Consumer_Cart;
end;
$$;

create table Provider
(
	ID_Provider serial not null constraint PK_Provider primary key,
	NameProvider varchar not null constraint UQ_NameProvider unique
);

create or replace procedure Provider_Insert(p_NameProvider varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from Provider
	where NameProvider = p_NameProvider;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		INSERT into Provider(NameProvider)
			values(p_NameProvider);
	end if;
	end;
$$;

create or replace procedure Provider_Update(p_ID_Provider int, p_TypeName varchar(100))
language plpgsql
as $$
	DECLARE have_record int:= count(*) from Provider
	where NameProvider = p_NameProvider;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным названием уже существует в таблице!';
	else 
		UPDATE Provider set
		NameProvider = p_NameProvider
			where 
			ID_Provider = p_ID_Provider;
	end if;
	end;
$$;

create or replace procedure Provider_Delete(p_ID_Provider int)
language plpgsql
as $$
	DECLARE have_record int:= count(*) from Delivery
	where Provider_ID = p_ID_Provider;
	begin
	if have_record > 0 then
		raise exception 'Роль с указанным id зайдействован';
	else 
		Delete from Provider 
			where ID_Provider = p_ID_Provider;
	end if;
	end;
$$;

create table Warehouse
(
	ID_Warehouse serial not null constraint PK_Warehouse primary key,
	Warehouse_Cell varchar(5) not null constraint CH_Warehouse_Cell check (Warehouse_Cell similar to '[A-Z]{2}[0-9]{3}'),
	Product_Availability boolean not null	
);

create or replace procedure Warehouse_Insert(p_Warehouse_Cell varchar(5), p_Product_Availability boolean)
language plpgsql
as $$
Declare have_record int := count(*)from Warehouse
where Warehouse_Cell=p_Warehouse_Cell;
begin
	if have_record>0 then
		raise exception 'Такая ячейка уже есть';
	else
		insert into Warehouse(Warehouse_Cell, Product_Availability)
		values(p_Warehouse_Cell, p_Product_Availability);
	end if;
end;
$$;

create or replace procedure Warehouse_Update(p_ID_Warehouse int, p_Warehouse_Cell varchar(5), p_Product_Availability boolean)
language plpgsql
as $$
Declare have_record int := count(*)from Warehouse
where Warehouse_Cell=p_Warehouse_Cell and Product_Availability=p_Product_Availability;
begin
	if have_record>0 then
		raise exception 'Такая ячейка уже есть';
	else
		UPDATE Warehouse Set
		Warehouse_Cell = p_Warehouse_Cell,
		Product_Availability = p_Product_Availability
			where 
			ID_Warehouse = p_ID_Warehouse;
	end if;
end;
$$;

create or replace procedure Warehouse_Delete(p_ID_Warehouse int)
language plpgsql
as $$
Declare have_record int := count(*)from Location_in_Warehouse
where Warehouse_Id=p_ID_Warehouse;
begin
	if have_record>0 then
		raise exception 'Данная ячейка используется в Location_in_Warehouse';
	else
		Delete from Warehouse 
			where 
				ID_Warehouse=p_ID_Warehouse;
	end if;
end;
$$;

create table LocationWarehouse
(
	ID_LocationWarehouse serial not null constraint PK_LocationWarehouse primary key,
	Product_ID int not null references Product(ID_Product),
	Warehouse_ID int not null references Warehouse(ID_Warehouse),
	Quantity_Of_Goods_on_Warehouse int not null constraint CH_Quantity_Of_Goods_on_Warehouse check(Quantity_Of_Goods_on_Warehouse>=0),
	Profile_ID int not null references Profile(ID_Profile)
);

create or replace procedure LocationWarehouse_Insert(p_Product_ID int, p_Warehouse_ID int, p_Quantity_Of_Goods_on_Warehouse int, p_Profile_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from LocationWarehouse
where Product_ID=p_Product_ID and p_Warehouse_ID=p_Warehouse_ID 
and Quantity_Of_Goods_on_Warehouse=p_Quantity_Of_Goods_on_Warehouse
and Profile_ID=p_Profile_ID;
begin
	if have_record>0 then
		raise exception 'Такое размещение на складе уже существует';
	else
		insert into LocationWarehouse(Product_ID, Warehouse_ID, Quantity_Of_Goods_on_Warehouse, Profile_ID)
		values(p_Product_ID, p_Warehouse_ID, p_Quantity_Of_Goods_on_Warehouse, p_Profile_ID);
	end if;
end;
$$;

create or replace procedure LocationWarehouse_Update(p_ID_LocationWarehouse int, p_Product_ID int, p_Warehouse_ID int, p_Quantity_Of_Goods_on_Warehouse int, p_Profile_ID int)
language plpgsql
as $$
Declare have_record int := count(*)from LocationWarehouse
where Product_ID=p_Product_ID and p_Warehouse_ID=p_Warehouse_ID and Quantity_Of_Goods_on_Warehouse= p_Quantity_Of_Goods_on_Warehouse and Profile_ID = p_Profile_ID;
begin
	if have_record>0 then
		raise exception 'Такое размещение на складе уже существует';
	else
		UPDATE LocationWarehouse Set
			Product_ID = p_Product_ID,
			Warehouse_ID = p_Warehouse_ID,
			Quantity_Of_Goods_on_Warehouse = p_Quantity_Of_Goods_on_Warehouse,
			Profile_ID = p_Profile_ID
			where 
				ID_LocationWarehouse = p_ID_LocationWarehouse;
	end if;
end;
$$;

create or replace procedure LocationWarehouse_Delete(p_ID_LocationWarehouse int)
language plpgsql
as $$
begin
	delete from LocationWarehouse 
		where ID_LocationWarehouse = p_ID_LocationWarehouse;
end;
$$;

create table Delivery
(
	ID_Delivery serial not null constraint PK_Delivery primary key,
	Warehouse_ID int not null references Warehouse(ID_Warehouse),
	Profile_ID int not null references Profile(ID_Profile),
	Product_ID int not null references Product(ID_Product),
	Provider_ID int not null references Provider(ID_Provider)
);


create table Product_History
(
	ID_Product_History SERIAL not null constraint PK_Product_History primary key,
	Status_Record varchar not null,
	Product_Info varchar not null,
	Characteristicz_Info varchar not null,
	Date_Create timestamp null default (now()::timestamp)
);


-------------------------------------
--------Функции для триггеров--------
-------------------------------------

create or replace function fc_History_Insert()
returns trigger 
as $$
	begin
		insert into Product_History(Status_Record, Product_Info, Characteristicz_Info)
		values('Добавлена новая запись',
			   (select Product_Name||' '||ProductPrice||'руб.' from Product where id_product=NEW.product_id),
			   (select 'Характеристика: '||name_characteristicz from Characteristicz where ID_Characteristicz=NEW.characteristicz_id));
		return new;
	end;
$$
language plpgsql;


create or replace function fc_History_Update()
returns trigger 
as $$
	begin
		insert into Product_History(Status_Record, Product_Info, Characteristicz_Info)
		values('Изменение записи',
			   (select Product_Name||' '||ProductPrice||'руб.' from Product where ID_Product=NEW.product_id),
			   (select 'Характеристика: '||name_characteristicz from Characteristicz where ID_Characteristicz=NEW.characteristicz_id));
		return new;
	end;
$$
language plpgsql;

create or replace function fc_History_Delete()
returns trigger 
as $$
	begin
		insert into Product_History(Status_Record, Product_Info, Characteristicz_Info)
		values ('Запись удалена', 
			   (select Product_Name||' '||ProductPrice||'руб.' from Product where ID_Product=OLD.product_id),
			   (select 'Характеристика: '||name_characteristicz from Characteristicz where ID_Characteristicz=OLD.characteristicz_id));
		return OLD;
	end;
$$
language plpgsql;

------------------------
--------Триггеры--------
------------------------

create trigger tg_History_insert
after insert on productcharacteristicz
for each row
execute procedure fc_History_insert();

create trigger tg_History_update
after update on productcharacteristicz
for each row
execute procedure fc_History_update();

create trigger tg_History_delete
before delete on productcharacteristicz
for each row
execute procedure fc_History_delete();


-----------------------
--------Функции--------
-----------------------


create or replace function Average_Сheck_Amount(p_Start_Period date, p_End_Period date)
returns decimal(38,2)
language plpgsql 
as $$
	declare count_money decimal(38,2);
	begin
	Select Avg(Total_Sum) into count_money
		from Product_Check 
			where Purchase_Date between p_Start_Period and p_End_Period;
	return count_money;
	end;
$$;

create or replace function Frequently_Bought_Product(p_Start_Period date, p_End_Period date)
returns varchar
language plpgsql 
as $$
	declare f_product varchar;
	begin
	Select Product_Name ||' '||count(QUANTITY_OF_PRODUCT) into f_product
	from Consumer_cart 
	right join Product on ID_Product = Product_ID 
	inner join Product_Check on id_product_check = product_check_id
		where Purchase_Date between p_Start_Period and p_End_Period
		group by PRODUCT_NAME
		Order by PRODUCT_NAME DESC
		Limit 1; 
	return f_product;
	end;	
$$;

create or replace function Shop_Revenue(p_Shop_Id int, p_Start_Period date, p_End_Period date)
returns decimal(38,2)
language plpgsql 
as $$
	declare count_money decimal(38,2);
	begin
	Select Sum(Total_Sum) into count_money
		from Product_Check 
		inner join Shop on id_shop = shop_id
			where shop_id = p_Shop_Id and Purchase_Date between p_Start_Period and p_End_Period;
	return count_money;
	end;
$$;

