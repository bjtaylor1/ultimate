create table TestORMData
(
Id integer primary key,
IntVal int,
BigIntVal int,
StringVal text,
EnumIntVal int,
EnumStringVal text,
FloatVal real,
DecimalVal real,
DateTimeVal text
);

insert into TestORMData(Id, IntVal, BigIntVal, StringVal, EnumIntVal, EnumStringVal, FloatVal, DecimalVal, DateTimeVal)
values(1, 715, 123456789123456789, 'hello', 1, 'EnumVal1', 123.45, 456.789, '2020-03-10 11:45');

insert into TestORMData(Id, IntVal, BigIntVal, StringVal, EnumIntVal, EnumStringVal, FloatVal, DecimalVal, DateTimeVal)
values(2, 42, 456789123456789123, 'goodbye', 2, 'EnumVal2', 456.78, 152.47, '1996-02-14 07:15');

insert into TestORMData(Id, IntVal)
values(3, 12 /* rest null */);