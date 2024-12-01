import type { ICategory } from "../../../types";
import { ProductList } from "../ProductList";

const categories: ICategory[] = [
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

export default function AllProducts() {
  return (
    <>
      <form
        method="get"
        action="/"
        className="d-flex justify-content-center my-4"
      >
        <div
          className="input-group"
          style={{ width: "700px", maxWidth: "100%" }}
        >
          <input
            type="text"
            name="searching"
            className="form-control"
            placeholder="Search products using product name or ID"
            style={{ border: "none", borderRadius: "15px 0 0 15px" }}
          />
          <button
            value="search"
            className="btn btn-primary"
            type="submit"
            style={{ border: "none", borderRadius: "0 15px 15px 0" }}
          >
            Search
          </button>
        </div>
      </form>

      <CategoryList categories={categories} />

      <ProductList />
    </>
  );
}

const CategoryList = ({ categories }: { categories: ICategory[] }) => {
  return (
    <section id="categoryLinks">
      <div className="row gap-2">
        {categories.map((c) => (
          <Category category={c} key={c.name} />
        ))}
      </div>
    </section>
  );
};

const Category = ({ category }: { category: ICategory }) => {
  return (
    <a
      href={`/?category=${category.name}`}
      className="col category-link text-center py-4 block"
    >
      <img src={category.imageUrl} className="icon" alt="Fruits" />
      <h5>{category.name}</h5>
    </a>
  );
};
