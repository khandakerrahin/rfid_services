package LogSenderApp;

import java.io.File;

public class ZipIt {
	public static void main( String[] args )
    {
		String OUTPUT_ZIP_FOLDER = "D:\\data\\log_zips\\";
	    String SOURCE_FOLDER = "D:\\data\\applicationLogs";
    	FileZipper appZip = new FileZipper(SOURCE_FOLDER);
    	File directory = new File(OUTPUT_ZIP_FOLDER);
    	if (! directory.exists()){
            directory.mkdirs();
        }
    	while(true) {
    		appZip.generateFileList(new File(SOURCE_FOLDER));
    		appZip.zipIt(OUTPUT_ZIP_FOLDER);
    		System.out.print(".");
    		appZip.fileList.clear();
    		try {
				Thread.sleep(3000);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
    	}
    }
}
