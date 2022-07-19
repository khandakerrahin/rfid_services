package ACCLogDataExtractor;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.file.FileAlreadyExistsException;
import java.nio.file.FileSystems;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

import com.mysql.jdbc.exceptions.jdbc4.MySQLIntegrityConstraintViolationException;

public class FileUnzipper {
	String OUTPUT_FOLDER;
	String SOURCE_FOLDER;
	String TARGET_FOLDER;
	String CORRUPT_FOLDER;
	String DUPLICATE_FOLDER;

	String url;
	String user;
	String password;

	Connection CONN;
	PreparedStatement PSTMNT;
	Statement STMNT;

	boolean conException = false;

	FileUnzipper(String out, String src, String tar, String cor, String dup, String u, String usr, String pw) {
		OUTPUT_FOLDER = out;
		SOURCE_FOLDER = src;
		TARGET_FOLDER = tar;
		CORRUPT_FOLDER = cor;
		DUPLICATE_FOLDER = dup;
		url = u;
		user = usr;
		password = pw;
	}

	public void initiate() throws Exception {
		String filName;
		File folder = new File(SOURCE_FOLDER);
		File[] listOfFiles = folder.listFiles();
		for (int i = 0; i < listOfFiles.length; i++) {
			if (listOfFiles[i].isFile()) {
				filName = listOfFiles[i].getName();
				if (filName.endsWith(".zip") || filName.endsWith(".ZIP")) {
					unZipFile(listOfFiles[i]);
				}
			}
		}
	}

	public boolean connectDB() {
		boolean retval = false;

		try {
			Class.forName("com.mysql.jdbc.Driver");
			CONN = DriverManager.getConnection(url, user, password);
			retval = true;
		} catch (Exception e) {
			e.printStackTrace();
			retval = false;
		}
		return retval;
	}

	public void insertFileToDB(String filename, int status) throws SQLException {
		String fileNameQuery = "insert into log_info (filename,status) values (?,?)";
		PSTMNT = CONN.prepareStatement(fileNameQuery);
		PSTMNT.setString(1, filename);
		PSTMNT.setInt(2, status);
		PSTMNT.executeUpdate();
	}

	public void unZipFile(File zipFile) throws Exception {
		byte[] buffer = new byte[1024];

		try {
			// create output directory is not exists
			File folder = new File(OUTPUT_FOLDER);
			if (!folder.exists()) {
				folder.mkdir();
			}

			// get the zip file content
			FileInputStream fis = new FileInputStream(zipFile);
			ZipInputStream zis = new ZipInputStream(fis);
			// get the zipped file list entry
			ZipEntry ze = zis.getNextEntry();

			while (ze != null) {
				String fileName = ze.getName();
				File newFile = new File(OUTPUT_FOLDER + File.separator + fileName);

				// create all non exists folders
				// else you will hit FileNotFoundException for compressed folder
				new File(newFile.getParent()).mkdirs();

				FileOutputStream fos = new FileOutputStream(newFile);

				int len;
				try {
					while ((len = zis.read(buffer)) > 0) {
						fos.write(buffer, 0, len);
					}
					fos.close();
					ze = zis.getNextEntry();
					fis.close();
					System.out.println("unzipped : " + zipFile.getName());
					try {
						File directory = new File(TARGET_FOLDER);
						if (!directory.exists()) {
							directory.mkdirs();
						}
						Path movefrom = FileSystems.getDefault()
								.getPath(SOURCE_FOLDER + File.separator + zipFile.getName());
						Path target = FileSystems.getDefault()
								.getPath(TARGET_FOLDER + File.separator + zipFile.getName());

						insertFileToDB(zipFile.getName().replace(".zip", ".txt"), 0);

						Files.deleteIfExists(Paths.get(TARGET_FOLDER + File.separator + zipFile.getName()));
						Files.move(movefrom, target);

						System.out.println("moved : " + zipFile.getName());
						System.out.println();
					} catch (MySQLIntegrityConstraintViolationException e) {
						System.out.println("unzipped failed: " + zipFile.getName());
						e.printStackTrace();
						fos.close();
						fis.close();
						try {
							File directory = new File(DUPLICATE_FOLDER);
							if (!directory.exists()) {
								System.out.println("dir does not exist");
								directory.mkdirs();
							}
							Path movefrom = FileSystems.getDefault()
									.getPath(SOURCE_FOLDER + File.separator + zipFile.getName());
							Path target = FileSystems.getDefault()
									.getPath(DUPLICATE_FOLDER + File.separator + zipFile.getName());

							Files.deleteIfExists(Paths.get(DUPLICATE_FOLDER + File.separator + zipFile.getName()));
							Files.move(movefrom, target);
							System.out.println("moved : " + zipFile.getName());
							System.out.println();
							ze = null;
						} catch (Exception ex) {
							ex.printStackTrace();
						}
						e.printStackTrace();
					} catch (Exception e) {
						e.printStackTrace();
						conException = true;
						throw e;
					}
				} catch (Exception e) {
					System.out.println("unzipped failed: " + zipFile.getName());
					e.printStackTrace();
					fos.close();
					fis.close();
					if (conException) {
						conException = false;
						throw e;
					} else {
						try {
							File directory = new File(CORRUPT_FOLDER);
							if (!directory.exists()) {
								System.out.println("dir does not exist");
								directory.mkdirs();
							}
							Path movefrom = FileSystems.getDefault()
									.getPath(SOURCE_FOLDER + File.separator + zipFile.getName());
							Path target = FileSystems.getDefault()
									.getPath(CORRUPT_FOLDER + File.separator + zipFile.getName());

							insertFileToDB(zipFile.getName().replace(".zip", ".txt"), 5);
							Files.deleteIfExists(Paths.get(CORRUPT_FOLDER + File.separator + zipFile.getName()));
							Files.move(movefrom, target);

							System.out.println("moved : " + zipFile.getName());
							System.out.println();
							ze = null;
						} catch (Exception ex) {
							ex.printStackTrace();
						}
					}
				}
			}
			zis.closeEntry();
			zis.close();
		} catch (IOException ex) {
			ex.printStackTrace();
		}
	}
}