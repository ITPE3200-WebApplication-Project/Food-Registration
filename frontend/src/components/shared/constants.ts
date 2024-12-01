export const PRODUCT_CATEGORIES = [
  "Fruits",
  "Vegetables",
  "Meat",
  "Fish",
  "Dairy",
  "Grains",
  "Beverages",
  "Snacks",
  "Other",
] as const;

export const NUTRITION_SCORES = ["A", "B", "C", "D", "E"] as const;

export type ProductCategory = (typeof PRODUCT_CATEGORIES)[number];
export type NutritionScore = (typeof NUTRITION_SCORES)[number];
