interface ICategory {
  name: string;
  imageUrl: string;
}
interface IProduct {
  id: number;
  name: string;
  description: string;
  category: string;
  imageUrl: string;
  nutritionScore: string;
  producer: string;
}

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
const products: IProduct[] = [
  {
    id: 1,
    name: "Apple",
    description: "Fresh red apples from local orchards",
    category: "Fruits",
    imageUrl: "/images/products/apple.jpg",
    nutritionScore: "A",
    producer: "Local Farms Co",
  },
  {
    id: 2,
    name: "Whole Wheat Bread",
    description: "Freshly baked whole grain bread",
    category: "Bakery Foods",
    imageUrl: "/images/products/bread.jpg",
    nutritionScore: "B",
    producer: "Healthy Bakery",
  },
  {
    id: 3,
    name: "Milk",
    description: "Fresh whole milk",
    category: "Dairy",
    imageUrl: "/images/products/milk.jpg",
    nutritionScore: "A",
    producer: "Happy Cows Dairy",
  },
  {
    id: 4,
    name: "Chicken Breast",
    description: "Lean chicken breast fillets",
    category: "Meats",
    imageUrl: "/images/products/chicken.jpg",
    nutritionScore: "A",
    producer: "Quality Meats Inc",
  },
  {
    id: 5,
    name: "Carrot",
    description: "Fresh organic carrots",
    category: "Vegetables",
    imageUrl: "/images/products/carrot.jpg",
    nutritionScore: "A",
    producer: "Organic Farms",
  },
  {
    id: 6,
    name: "Orange Juice",
    description: "100% pure squeezed orange juice",
    category: "Drinks",
    imageUrl: "/images/products/juice.jpg",
    nutritionScore: "C",
    producer: "Fresh Juice Co",
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

      {/* <!--Table-->
<div class="container-fluid mt-4">
        <div class="text-center py-5">
            <i class="bi bi-search text-muted" style="font-size: 4rem;"></i>
            <h3 class="mt-3 text-muted">No Products Found</h3>
            <p class="text-muted mb-4">
                @if (ViewData["Getproductlist"] != null)
                {
                    <span>No products match your search for "@ViewData["Getproductlist"]"</span>
                }
                else
                {
                    <span>Try searching for products or browse by category above</span>
                }
            </p>
            @if (ViewData["Getproductlist"] != null)
            {
                <a href="/" class="btn btn-primary rounded-pill px-4">
                    <i class="bi bi-arrow-repeat me-2"></i>Clear Search
                </a>
            }
        </div> */}

      <ProductList products={products} />
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
      className="col category-link text-center bg-white py-4 block"
    >
      <img src={category.imageUrl} className="icon" alt="Fruits" />
      <h5>{category.name}</h5>
    </a>
  );
};

const ProductList = ({ products }: { products: IProduct[] }) => {
  return (
    <div className="row g-4">
      {products.map((product) => (
        <ProductCard key={product.id} product={product} />
      ))}
    </div>
  );
};

const ProductCard = ({ product }: { product: IProduct }) => {
  return (
    <>
      <div className="col-12 col-sm-6 col-md-4 col-lg-3">
        <div className="product-card">
          <img
            className="product-image"
            src={product.imageUrl}
            alt={product.name}
          />
          <div className="product-content">
            <h5 className="product-title">{product.name}</h5>
            <div className="product-footer">
              {/* @if (!string.IsNullOrEmpty(product.NutritionScore))
                {
                    <img className="nutrition-score"
                        src="@Url.Content($"~/images/Nutri/Nutri-score-{product.NutritionScore}.svg.png")"
                        alt="Nutrition Score @product.NutritionScore" />
                }
                else
                {
                    <span>N/A</span>
                } */}
              <a href="" className="btn btn-primary rounded-pill">
                Read More
              </a>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};
