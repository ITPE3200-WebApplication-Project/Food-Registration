export interface ICreateProductDTO {
  name: string;
  nutritionScore: string;
  producerId: number;
  imageBase64: string;
  imageFileExtension: string;
  carbohydrates?: number;
  category?: string;
  description?: string;
  fat?: number;
  protein?: number;
  calories?: number;
}

export type IUpdateProductDTO = {
  productId: number;
  name: string;
  description: string;
  category: string;
  nutritionScore: string;
  producerId: number;
  imageBase64?: string;
  imageFileExtension?: string;
  calories: number;
  carbohydrates?: number;
  fat: number;
  protein: number;
};

export interface ICreateProducerDTO {
  name: string;
  description: string;
  imageBase64: string;
  imageFileExtension: string;
}

export type IUpdateProducerDTO = {
  producerId: number;
  name: string;
  description: string;
  imageBase64?: string;
  imageFileExtension?: string;
};

export type IRegisterDTO = {
  email: string;
  password: string;
  confirmPassword: string;
};
