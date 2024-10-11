if [ $# -ne 1 ]
then
    echo "Erreur : Entrer le nom du fichier à traiter !!!"
    exit 1
fi
fic_tmp="fic_tmp"
ficbody=$1
cat $ficbody | sed -e 's/;a/;aa/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;b/;bb/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;c/;cc/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;d/;dd/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;e/;ee/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;f/;ff/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;g/;gg/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;h/;hh/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;i/;ii/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;j/;jj/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;k/;kk/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;l/;ll/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;m/;mm/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;n/;nn/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;o/;oo/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;p/;pp/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;q/;qq/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;r/;rr/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;s/;ss/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;t/;tt/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;u/;uu/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;v/;vv/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;w/;ww/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;x/;xx/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;y/;yy/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;z/;z1/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;(/;za/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;)/;zb/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;\[/;zc/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;]/;zd/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;{/;ze/g' > $fic_tmp
mv $fic_tmp $ficbody
cat $ficbody | sed -e 's/;}/;zf/g' > $fic_tmp
mv $fic_tmp $ficbody
