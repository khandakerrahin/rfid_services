package ACCLogDataExtractor;

import java.sql.Connection;

import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.io.File;
import java.io.FileNotFoundException;
import java.nio.file.Files;
import java.nio.file.Paths;

public class DBUploader {
	Connection CONN;
	PreparedStatement PSTMNT;
	Statement STMNT;
	String SOURCE_FOLDER;
	String url;
	String user;
	String password;
	
	DBUploader(String src,String u,String usr,String pw){
		SOURCE_FOLDER = src;
		url = u;
		user = usr;
		password = pw;
	}
	
	public void initiate() throws Exception {
		recursiveFolderUpload(SOURCE_FOLDER);
	}
	
	public void uploadFileData(String filename) throws Exception {
		int fileID = -1;
		int fileStatus=-1;
		
		try {
			fileStatus = fetchFileStatusfromDB(filename);
			if(fileStatus==0) {
				try {
					updateFileStatus(filename,-1);
					fileID = fetchFileIDfromDB(filename);
					
					String loadQuery = "LOAD DATA LOCAL INFILE '" + SOURCE_FOLDER+filename
						    + "' IGNORE INTO TABLE antenna_diagnostics FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"'"
						    + " LINES TERMINATED BY '\n' " + "(@Antenna_ID,@Error_Time,@Error_Code,@Error_Message)" + " SET fileID='"+fileID+"',"
							+ "Antenna_ID=trim(BOTH '\\b' from trim(BOTH '\\n' from trim(BOTH '\\r' from trim(BOTH '\\t' from trim(' ' from @Antenna_ID))))),"
							+ "Error_Time=trim(BOTH '\\b' from trim(BOTH '\\n' from trim(BOTH '\\r' from trim(BOTH '\\t' from trim(' ' from @Error_Time))))),"
							+ "Error_Code=trim(BOTH '\\b' from trim(BOTH '\\n' from trim(BOTH '\\r' from trim(BOTH '\\t' from trim(' ' from @Error_Code))))),"
							+ "Error_Message=trim(BOTH '\\b' from trim(BOTH '\\n' from trim(BOTH '\\r' from trim(BOTH '\\t' from trim(' ' from @Error_Message))))) ";
					
					STMNT = CONN.createStatement();
					STMNT.executeQuery(loadQuery);			
					STMNT.close();
					System.out.println("inserted : "+filename);
					updateFileStatus(filename,1);
					try{
						Files.deleteIfExists(Paths.get(SOURCE_FOLDER+filename));
						System.out.println("deleted : "+filename);
						updateFileStatus(filename,2);
					}catch(Exception e) {
						System.out.println("deletion failed : " + filename);
						e.printStackTrace();
						throw e;
					}
				} catch (SQLException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
					throw e;
				}
			}
			else if(fileStatus==1) {
				try{
					Files.deleteIfExists(Paths.get(SOURCE_FOLDER+filename));
					System.out.println("deleted : "+filename);
					updateFileStatus(filename,2);
				}catch(Exception e) {
					System.out.println("deletion failed : " + filename);
					e.printStackTrace();
					throw e;
				}
			}
			else if(fileStatus==-1) {
				try {
					//fileID = fetchFileIDfromDB(filename);
					//deleteDBentryByID(fileID);
					updateFileStatus(filename,0);
					uploadFileData(filename);
				}catch(SQLException e) {
					e.printStackTrace();
					throw e;
				}
			}
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			throw e;
		}
	}

	int fetchFileIDfromDB(String flname) throws SQLException {
		int id = -1;
		ResultSet rs;
		String fetchFileID="select id from log_info where filename=?";
		PSTMNT=CONN.prepareStatement(fetchFileID);  
		PSTMNT.setString(1,flname);  
		rs = PSTMNT.executeQuery();
		while(rs.next()) {
			id = rs.getInt("id");
		}
		PSTMNT.close();
		rs.close();
		//System.out.println("id = "+id);
		return id;
	}
	
	int fetchFileStatusfromDB(String flname) throws SQLException {
		int status = 5;
		ResultSet rs;
		String fetchFileStatus="select status from log_info where filename=?";
		PSTMNT=CONN.prepareStatement(fetchFileStatus);  
		PSTMNT.setString(1,flname);  
		rs = PSTMNT.executeQuery();
		while(rs.next()) {
			status = rs.getInt("status");
		}
		PSTMNT.close();
		rs.close();
		//System.out.println("status = "+status);
		return status;
	}
	
	void updateFileStatus(String flname, int sts) throws SQLException {
		String updateStatus="update log_info set status=? where filename=?";
		PSTMNT=CONN.prepareStatement(updateStatus);  
		PSTMNT.setInt(1,sts);
		PSTMNT.setString(2,flname);  
		PSTMNT.executeUpdate();	
		PSTMNT.close();
		//System.out.println("status updated : "+sts);
	}
	
	void deleteDBentryByID(int id) throws SQLException {
		String deleteEntries="delete from antenna_diagnostics where fileID=?";
		PSTMNT=CONN.prepareStatement(deleteEntries);  
		PSTMNT.setInt(1,id);  
		PSTMNT.executeUpdate();	
		PSTMNT.close();
		System.out.println("enrties deleted");
	}
	
	void recursiveFolderUpload(String sourcePath) throws Exception {
	        File sourceFile = new File(sourcePath);
	        if (sourceFile.isFile()) {
	        	if (!sourceFile.getName().startsWith(".")) {
	                try{
	                	uploadFileData(sourceFile.getName());
		            }catch(Exception e){
	             		e.printStackTrace();
	             		throw e;
	             	}
	            }
	        } else {
	        	//System.out.println("inside else " + sourceFile.getName());
	            File[] files = sourceFile.listFiles();

	            if (files != null && !sourceFile.getName().startsWith(".")) {
	                for (File f: files) {
	                    recursiveFolderUpload(f.getAbsolutePath());
	                }

	            }
	        }

	    }
	
	/**
	 * 
	 * @return boolean
	 * @throws Exception 
	 */
	public boolean connectDB() throws Exception {
		boolean retval = false;

		try {
			Class.forName("com.mysql.jdbc.Driver");
			CONN = DriverManager.getConnection(url, user, password);
			retval = true;
		} catch (Exception e) {
			e.printStackTrace();
			retval = false;
			throw e;
		}
		return retval;
	}

	public void upd_table() throws SQLException {
		
	}
}
