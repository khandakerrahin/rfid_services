package ACCZipDataExtractor;

public class UploadData {
	public static void main(String[] args) throws Exception {
		String SOURCE_FOLDER = "/home/shaker/data_zipsExtracts/";
		
		String url = "jdbc:mysql://10.10.1.1:3306/schools?useUnicode=true&characterEncoding=utf-8";
		String user = "shaker";
		String password = "app@Spider&1";
		
		DBUploader dbu = new DBUploader(SOURCE_FOLDER,url,user,password);
		
		if (dbu.connectDB()) {
			System.out.println("DB connection successful");
			while (true) {
				try {
					dbu.initiate();
					System.out.print(".");
					Thread.sleep(3000);
				} catch (Exception e) {
					e.printStackTrace();
					if (dbu.connectDB()) {
						System.out.println("DB connection re-established");
					} else {
						System.out.println("Database connection failed");
						try {
							Thread.sleep(10000);
						} catch (InterruptedException e1) {
							// TODO Auto-generated catch block
							e1.printStackTrace();
						}
					}
				}
			}
		} else {
			System.out.println("Database connection failed");
		}
	}
}
