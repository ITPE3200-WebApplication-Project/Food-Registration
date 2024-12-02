import type { ICategory } from "../../../types";
import { Category } from "./Category";

const categories: ICategory[] = [
  {
    name: "All",
    imageUrl: "/images/icons/basket.png",
  },
  {
    name: "Fruits",
    imageUrl: "/images/icons/basket.png",
  },
  {
    name: "Vegetables",
    imageUrl: "/images/icons/vegetable.png",
  },
  {
    name: "Meats",
    imageUrl: "/images/icons/proteins.png",
  },
  {
    name: "Bakery Foods",
    imageUrl: "/images/icons/breads.png",
  },
  {
    name: "Dairy",
    imageUrl: "/images/icons/daily-products.png",
  },
  {
    name: "Drinks",
    imageUrl: "/images/icons/soft-drink.png",
  },
  {
    name: "Other",
    imageUrl: "/images/icons/fast-food.png",
  },
];

export function CategoryList() {
  return (
    <section id="categoryLinks">
      <div className="row gap-2">
        {categories.map((c) => (
          <Category category={c} key={c.name} />
        ))}
      </div>
    </section>
  );
}
