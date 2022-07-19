package LogSenderApp;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.FileAlreadyExistsException;
import java.nio.file.FileSystems;
import java.nio.file.Files;
import java.nio.file.Path;
import com.jcraft.jsch.Channel;
import com.jcraft.jsch.ChannelSftp;
import com.jcraft.jsch.JSch;
import com.jcraft.jsch.Session;
import com.jcraft.jsch.SftpATTRS;
import com.jcraft.jsch.SftpException;

public class SFTPuploader {

    ChannelSftp channelSftp = null;
    Session session = null;
    Channel channel = null;
    String PATHSEPARATOR = "/";
    String SFTPWORKINGDIR; // Source Directory on SFTP server
    String LOCALDIRECTORY; // Local Target Directory
    String TARGETDIRECTORY;
  
    SFTPuploader(String workingdir,String localdir,String targetdir){
    	SFTPWORKINGDIR = workingdir;
        LOCALDIRECTORY = localdir;
        TARGETDIRECTORY = targetdir;
    }
    
    public void getConnection(String SFTPUSER, String SFTPHOST, int SFTPPORT, String SFTPPASS) {
    	try {
	    	JSch jsch = new JSch();
	        session = jsch.getSession(SFTPUSER, SFTPHOST, SFTPPORT);
	        session.setPassword(SFTPPASS);
	        java.util.Properties config = new java.util.Properties();
	        config.put("StrictHostKeyChecking", "no");
	        session.setConfig(config);
	        session.setTimeout(90000);
	        session.connect();
	        channel = session.openChannel("sftp");
	        channel.connect();
	        channelSftp = (ChannelSftp) channel;
	        
	        channelSftp.cd(SFTPWORKINGDIR); // Change Directory on SFTP Server
	        System.out.println("connection created");
        }catch(Exception ex) {
        	ex.printStackTrace();
        }
    	
    }

    public void recursiveFolderUpload(String sourcePath, String destinationPath) throws SftpException, FileNotFoundException {
        
        File sourceFile = new File(sourcePath);
        if (sourceFile.isFile()) {
            
            // copy if it is a file
            channelSftp.cd(destinationPath);
            if (!sourceFile.getName().startsWith(".")) {
                FileInputStream fis = new FileInputStream(sourceFile);
            	channelSftp.put(fis, sourceFile.getName(), ChannelSftp.OVERWRITE);
                try {
					fis.close();
					System.out.println("uploaded : "+sourceFile.getName());
				} catch (IOException e1) {
					e1.printStackTrace();
				}
            	try{
	            	File directory = new File(TARGETDIRECTORY);
	            	if (! directory.exists()){
	                    directory.mkdirs();
	                }
	            	Path movefrom = FileSystems.getDefault().getPath(LOCALDIRECTORY + sourceFile.getName());
	            	Path target = FileSystems.getDefault().getPath(TARGETDIRECTORY + sourceFile.getName());
	            			
	            	Files.move(movefrom, target);
	            	
	            	System.out.println("moved : " + sourceFile.getName());
	            	System.out.println();
             	}catch(FileAlreadyExistsException e){
             		e.printStackTrace();
             	}
            	catch(Exception e){
             		e.printStackTrace();
             	}
            }
        } else {
            
            //System.out.println("inside else " + sourceFile.getName());
            File[] files = sourceFile.listFiles();

            if (files != null && !sourceFile.getName().startsWith(".")) {

                channelSftp.cd(destinationPath);
                SftpATTRS attrs = null;

                // check if the directory is already existing
                try {
                    attrs = channelSftp.stat(destinationPath + "/" + sourceFile.getName());
                } catch (Exception e) {
                    System.out.println(destinationPath + "/" + sourceFile.getName() + " not found");
                }

                // else create a directory
                if (attrs != null) {
                    //System.out.println("Directory exists IsDir=" + attrs.isDir());
                } else {
                    //System.out.println("Creating dir " + sourceFile.getName());
                    channelSftp.mkdir(sourceFile.getName());
                }

                for (File f: files) {
                    recursiveFolderUpload(f.getAbsolutePath(), destinationPath + "/" + sourceFile.getName());
                }

            }
        }

    }

}