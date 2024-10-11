export PGPASSWORD='shutweb'
psql -U postgres -d exemple -w <<EOF
\COPY testfile(i) FROM 'tmp.txt';
EOF

