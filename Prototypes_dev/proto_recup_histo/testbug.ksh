fic_tmp="fic_tmp"
>$fic_tmp
for i in 1 2 3 4 5 6 7
do
    #cat fi"$i"
    cat fi"$i" | awk -F";" '{if (NF==59) print $O";"; else if (NF==61) print susbtr($O,1,length($0)-1); else if (NF==60) print $0;}' > $fic_tmp
    #cat fi1 | awk -F";" '{if (NF==59) print $O";"; else if (NF==61) print substr($0,1,length($0)-1;}' > $fic_tmp
    cp -p $fic_tmp fi"$i"
done
