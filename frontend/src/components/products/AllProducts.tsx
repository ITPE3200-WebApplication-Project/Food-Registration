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

      <section id="categoryLinks">
        <div className="row gap-2">
          <a
            href=""
            className="col category-link text-center bg-white py-4 block"
          >
            <img src="" className="icon" alt="Fruits" />
            <h5>Fruits</h5>
          </a>
          <a
            href=""
            className="col category-link text-center bg-white py-4 block"
          >
            <img src="" className="icon" alt="Vegetables" />
            <h5>Vegetables</h5>
          </a>
          <a
            href=""
            className="col category-link text-center bg-white py-4 block"
          >
            <img src="" className="icon" alt="Meats" />
            <h5>Meats</h5>
          </a>
          <a
            href=""
            className="col category-link text-center bg-white py-4 block"
          >
            <img
              src="/images/icons/breads.png"
              className="icon"
              alt="Bakery Food"
            />
            <h5>Bakery Foods</h5>
          </a>
          <a
            href=""
            className="col category-link text-center bg-white py-4 block"
          >
            <img src="" className="icon" alt="Dairy" />
            <h5>Dairy</h5>
          </a>
          <a
            href=""
            className="col category-link text-center bg-white py-4 block"
          >
            <img src="" className="icon" alt="Drinks" />
            <h5>Drinks</h5>
          </a>
          <a
            href=""
            className="col category-link text-center bg-white py-4 block"
          >
            <img src="" className="icon" alt="Other" />
            <h5>Other</h5>
          </a>
        </div>
      </section>

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

      <ProductList products={[]} />
    </>
  );
}

interface Product {
  productId: number;
  name: string;
  imageUrl: string;
  nutritionScore: string;
}

const ProductList = ({ products }: { products: Product[] }) => {
  return (
    <div className="row g-4">
      {products.map((product) => (
        <ProductCard key={product.productId} product={product} />
      ))}
    </div>
  );
};

const ProductCard = ({ product }: { product: Product }) => {
  return (
    <>
      <div className="col-12 col-sm-6 col-md-4 col-lg-3">
        <div className="product-card">
          <img className="product-image" src="" alt="@product.Name" />
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
