docker exec -it 08812 bash
psql -p 5432 -U postgres -d testscale << EOF
select count(*) from public.testhlr;
COPY TESTHLR(i) TO 'C:\Users\hleterrier\Desktop\tutodocker\tata.txt' DELIMITER ';';
EOF
