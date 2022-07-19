package SenderApp;

public class Uploader {
	public static void main(String[] args) {
        
		String SFTPWORKINGDIR = "/home/shaker/schools_demo_files/"; // Source Directory on SFTP server
	    String LOCALDIRECTORY = "D:\\data\\data_zips\\"; // Local Target Directory
	    String TARGETDIRECTORY = "D:\\data\\data_archive\\";
		
		String SFTPHOST = "27.147.142.54";
        int SFTPPORT = 22;
        String SFTPUSER = "shaker"; // User Name
        String SFTPPASS = "Mashaker1193"; // Password
        
        SFTPuploader sup = new SFTPuploader(SFTPWORKINGDIR,LOCALDIRECTORY,TARGETDIRECTORY);
        
        sup.getConnection(SFTPUSER, SFTPHOST, SFTPPORT, SFTPPASS);

        while(true) {
        	try {
        		sup.recursiveFolderUpload(LOCALDIRECTORY,SFTPWORKINGDIR);
        		System.out.print(".");
        		Thread.sleep(3000);
	        }catch (Exception ex) {
	            ex.printStackTrace();
	            if(sup.session.isConnected() && sup.channel.isConnected()) {}
	            else
            	{
	            	System.out.println("connection lost.");
	            	System.out.println("reconnecting...");
	            	try {
	            		Thread.sleep(10000);
	    	        }catch (Exception e) {e.printStackTrace();}
	            	
	            	sup.getConnection(SFTPUSER, SFTPHOST, SFTPPORT, SFTPPASS);
            		
            		System.out.println("timeout: 10 seconds");
            	}
	        }
    	}
    }
}
