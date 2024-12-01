export interface ICreateProductDTO {
  name: string;
  nutritionScore: string;
  producerId: number;
  imageFile: File | null;
  carbohydrates?: number;
  category?: string;
  description?: string;
  fat?: number;
  protein?: number;
  calories?: number;
}
