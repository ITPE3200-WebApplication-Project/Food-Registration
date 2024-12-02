import { BASE_URL } from "./api";

export const getBase64 = async (
  file: File,
  cb: (result: string) => void
): Promise<void> => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
      cb(reader.result as string);
      resolve();
    };
    reader.onerror = function (error) {
      console.log("Error: ", error);
      reject(error);
    };
  });
};

export const getImageUrl = (path?: string) => {
  if (path && path.startsWith("/")) {
    return `${BASE_URL}${path}`;
  }
  return path;
};
