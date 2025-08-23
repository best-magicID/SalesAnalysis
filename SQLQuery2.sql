-- ������ 
select *	 
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel


-- ��������� �� ���������� ��� 
select NameModel, 
	   CountSoldModels, 
	   DateSaleModel
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel
where DATEPART(yy, DateSaleModel) = 2024


-- ��������� �� ���������� ��� 
select SUM(CountSoldModels) as �����
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel
where DATEPART(yy, DateSaleModel) = 2024


-- ��������� ����� ������ ������ �� ������������ ���
select SUM (CountSoldModels * Models.PriceModel ) as '���-��' 
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel
where DATEPART(yy, DateSaleModel) = 2024
and DatesSale.IdModel = 2


select SUM (CountSoldModels * Models.PriceModel ) as '���-��' 
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel
where DATEPART(MM, DateSaleModel) = 03
and DATEPART(YY, DateSaleModel) = 2023
and DatesSale.IdModel = 1