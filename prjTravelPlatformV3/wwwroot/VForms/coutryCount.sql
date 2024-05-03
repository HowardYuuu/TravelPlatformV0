select c.fCountry, COUNT(*) AS 'count'
from tVProduct as p
join tVCountry as c
on p.fCountryId = c.fId
group by c.fCountry