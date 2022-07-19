package LogSenderApp;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

public class FileZipper
{
    List<String> fileList;
    private String SOURCE_FOLDER;
	
    FileZipper(String src){
    	fileList = new ArrayList<String>();
    	SOURCE_FOLDER = src;
    }
	
    
    
    /**
     * Zip it
     * @param zipFile output ZIP file location
     */
    public void zipIt(String zipFile){

     byte[] buffer = new byte[1024];
    	
     try{
    	for(String file : this.fileList){
    		//Path p = Paths.get("D:\\data\\"+file);
    		//BasicFileAttributes attr = Files.readAttributes(p, BasicFileAttributes.class);

    		//System.out.println("creationTime: " + attr.creationTime());
    		//System.out.println("lastAccessTime: " + attr.lastAccessTime());
    		//System.out.println("lastModifiedTime: " + attr.lastModifiedTime());

    		//int TEN_MINUTES = 1 * 60 * 1000;
    		//long tenAgo = System.currentTimeMillis() - TEN_MINUTES;
    		File processCheck = new File(SOURCE_FOLDER + File.separator + file);
    		Boolean canBeDeleted = processCheck.renameTo(processCheck);
    		//System.out.println(canBeDeleted + " : here. . .");
    		//if (attr.creationTime().toMillis() < tenAgo) {
    		if(canBeDeleted && processCheck.length()>0) {
    			//System.out.println(file + " : is completely written.");
    		    //System.out.println("zipping : " + file);
    		    FileOutputStream fos = new FileOutputStream(zipFile+file.replace(".txt","")+".zip");
            	ZipOutputStream zos = new ZipOutputStream(fos);
            		
            	//System.out.println("Output to Zip : " + zipFile+file.replace(".txt","")+".zip");	
        		System.out.println("zipped : " + file +"   "+ processCheck.length()+"bytes");
        		ZipEntry ze= new ZipEntry(file);
            	zos.putNextEntry(ze);
                   
            	FileInputStream in = 
                           new FileInputStream(SOURCE_FOLDER + File.separator + file);
           	   
            	int len;
            	while ((len = in.read(buffer)) > 0) {
            		zos.write(buffer, 0, len);
            	}
                   
            	in.close();
            	zos.closeEntry();
            	zos.close();
            	boolean result = Files.deleteIfExists(Paths.get(SOURCE_FOLDER + File.separator + file));
            	
//            	if(result)
//            		System.out.println(file+" is deleted");
//            	else
//            		System.out.println(file+" is not deleted");
    		}
//    		else {
//    			System.out.println(file + " : is not completely written.");
//    		    System.out.println("skipping : " + file);
//    		}
    		
//    		System.out.println();
    	}
//    	System.out.println("Done");
    }catch(IOException ex){
       ex.printStackTrace();   
    }
   }
    
    /**
     * Traverse a directory and get all files,
     * and add the file into fileList  
     * @param node file or directory
     */
    public void generateFileList(File node){

    	//add file only
	if(node.isFile()){
		fileList.add(generateZipEntry(node.getAbsoluteFile().toString()));
	}
		
	if(node.isDirectory()){
		String[] subNote = node.list();
		for(String filename : subNote){
			generateFileList(new File(node, filename));
		}
	}
 
    }

    /**
     * Format the file path for zip
     * @param file file path
     * @return Formatted file path
     */
    private String generateZipEntry(String file){
    	return file.substring(SOURCE_FOLDER.length()+1, file.length());
    }
}