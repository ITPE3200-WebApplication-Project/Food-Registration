import { CategoryList } from "./CategoryList";
import { ProductList } from "./ProductList";

export default function Page() {
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

      <CategoryList />
      <ProductList />
    </>
  );
}
