package ACCZipDataExtractor;

public class UnzipIt {
	public static void main(String[]args) {
        
		String OUTPUT_FOLDER = "/home/shaker/data_zipsExtracts/";
	    String SOURCE_FOLDER = "/home/shaker/data_zips";
	    String TARGET_FOLDER = "/home/shaker/data_zipArchive";
	    String CORRUPT_FOLDER = "/home/shaker/data_corrupt";
	    String DUPLICATES_FOLDER = "/home/shaker/data_duplicates";
	    
	    String url = "jdbc:mysql://10.10.1.1:3306/schools?useUnicode=true&characterEncoding=utf-8";
		String user = "shaker";
		String password = "app@Spider&1";
	    
	    FileUnzipper fun = new FileUnzipper(OUTPUT_FOLDER,SOURCE_FOLDER,TARGET_FOLDER,CORRUPT_FOLDER,DUPLICATES_FOLDER,url,user,password);
	    
	    if (fun.connectDB()) {
			System.out.println("DB connection successful");
			while(true) {
	        	try {
	        		fun.initiate();
			        System.out.print(".");
		    		Thread.sleep(3000);
		        }catch (Exception ex) {
		            ex.printStackTrace();
		            if (fun.connectDB()) {
						System.out.println("DB connection re-established");
					} else {
						System.out.println("Database connection failed");
						try {
							Thread.sleep(10000);
						} catch (InterruptedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
					}
		         }
	        }
		} else {
			System.out.println("Database connection failed");
		}
    }
}
