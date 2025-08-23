-- Запрос 
select *	 
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel


-- Получение за конкретный год 
select NameModel, 
	   CountSoldModels, 
	   DateSaleModel
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel
where DATEPART(yy, DateSaleModel) = 2024


-- Получение за конкретный год 
select SUM(CountSoldModels) as колво
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel
where DATEPART(yy, DateSaleModel) = 2024


-- Получение суммы продаж модели за определенный год
select SUM (CountSoldModels * Models.PriceModel ) as 'кол-во' 
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel
where DATEPART(yy, DateSaleModel) = 2024
and DatesSale.IdModel = 2


select SUM (CountSoldModels * Models.PriceModel ) as 'кол-во' 
from DatesSale
right join Models on Models.IdModel = DatesSale.IdModel
where DATEPART(MM, DateSaleModel) = 03
and DATEPART(YY, DateSaleModel) = 2023
and DatesSale.IdModel = 1