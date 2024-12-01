export interface IProduct {
  productId: number;
  name: string;
  description: string;
  category: string;
  imageUrl: string;
  nutritionScore: string;
  producerId?: number;
  producer?: IProducer;
  calories?: number;
  carbohydrates?: number;
  fat?: number;
  protein?: number;
}

export interface IProducer {
  producerId: number;
  ownerId: string;
  name: string;
  description?: string;
  imageUrl?: string;
}

export interface ICategory {
  name: string;
  imageUrl: string;
}
