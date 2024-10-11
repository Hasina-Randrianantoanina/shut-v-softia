package com.example;

import org.apache.commons.net.ftp.FTPClient;
import org.apache.commons.net.ftp.FTPReply;
import org.apache.commons.net.PrintCommandListener;
import org.apache.commons.net.ProtocolCommandEvent;
import org.apache.commons.net.ProtocolCommandListener;
import java.io.*;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Properties;

public class ExtendedFTPClient {
    private static Properties prop = new Properties();
    static {
        try (InputStream input = ExtendedFTPClient.class.getClassLoader().getResourceAsStream("config.properties")) {
            prop.load(input);
        } catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    private static final String SERVER = prop.getProperty("ftp.server");
    private static final int PORT = Integer.parseInt(prop.getProperty("ftp.port"));
    private static final String USER = prop.getProperty("ftp.user");
    private static final String PASS = prop.getProperty("ftp.password");

    private static FTPClient ftpClient;

    public static void main(String[] args) {
        ftpClient = new FTPClient();
        ftpClient.addProtocolCommandListener(new PrintCommandListener(new PrintWriter(System.out), true));
        try {
            connect();
            getStatus();
            putRequest();
            getResult();
        } catch (IOException e) {
            System.out.println("Erreur: " + e.getMessage());
            e.printStackTrace();
        } finally {
            disconnect();
        }
    }

    private static void connect() throws IOException {
        System.out.println("Connexion au serveur FTP...");
        ftpClient.connect(SERVER, PORT);
        int replyCode = ftpClient.getReplyCode();
        if (!FTPReply.isPositiveCompletion(replyCode)) {
            throw new IOException("Échec de la connexion FTP : " + replyCode);
        }

        if (!ftpClient.login(USER, PASS)) {
            throw new IOException("Échec de l'authentification FTP");
        }

        ftpClient.enterLocalActiveMode();
        // ftpClient.enterLocalPassiveMode();
        System.out.println("Connecté au serveur FTP");
    }

    private static void getStatus() throws IOException {
        System.out.println("Téléchargement de STATUS.TXT...");
        File statusFile = new File("STATUS.txt");
        try (OutputStream outputStream = new BufferedOutputStream(new FileOutputStream(statusFile))) {
            if (ftpClient.retrieveFile("STATUS.TXT", outputStream)) {
                System.out.println("STATUS.TXT téléchargé avec succès");
            } else {
                throw new IOException("Échec du téléchargement de STATUS.TXT");
            }
        }
    }

    private static void putRequest() throws IOException {
        System.out.println("Création et upload de REQUETE.TXT...");
        SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy");
    
        Calendar cal = Calendar.getInstance();
        Date today = cal.getTime();
    
        cal.add(Calendar.DAY_OF_MONTH, -1);
        Date oneDayAgo = cal.getTime();
    
        String todayStr = sdf.format(today);
        String oneDayAgoStr = sdf.format(oneDayAgo);
    
        String content = "DEB=" + oneDayAgoStr + "\nFIN=" + todayStr + "\n";
    
        File localFile = new File("REQUETE.TXT");
        try (FileWriter writer = new FileWriter(localFile)) {
            writer.write(content);
        }
        System.out.println("REQUETE.TXT sauvegardé localement");
    
        try (InputStream inputStream = new ByteArrayInputStream(content.getBytes())) {
            if (ftpClient.storeFile("REQUETE.TXT", inputStream)) {
                System.out.println("REQUETE.TXT uploadé avec succès sur le serveur FTP");
            } else {
                throw new IOException("Échec de l'upload de REQUETE.TXT sur le serveur FTP");
            }
        }
    }

    private static void getResult() throws IOException {
        System.out.println("Téléchargement de RESULT.BIN...");
        boolean resultReceived = false;
    
        File resultFile = new File("RESULT.BIN");
        try (OutputStream outputStream = new BufferedOutputStream(new FileOutputStream(resultFile))) {
            if (ftpClient.retrieveFile("RESULT.BIN", outputStream)) {
                System.out.println("RESULT.BIN téléchargé avec succès");
                resultReceived = true;
            } else {
                System.out.println("RESULT.BIN non trouvé.");
            }
        }
    
        if (!resultReceived) {
            throw new IOException("Échec du téléchargement de RESULT.BIN");
        }
    }

    private static void disconnect() {
        try {
            if (ftpClient.isConnected()) {
                ftpClient.logout();
                ftpClient.disconnect();
                System.out.println("Déconnecté du serveur FTP");
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}