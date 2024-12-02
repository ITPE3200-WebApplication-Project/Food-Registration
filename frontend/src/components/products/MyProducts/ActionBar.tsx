export function ActionBar() {
  return (
    <div
      className="d-flex justify-content-between align-items-center"
      style={{ width: "100%", minHeight: "60px" }}
    >
      <a href="/products/create" className="btn btn-success rounded-pill px-3">
        <i className="bi bi-plus-circle"></i> Add New Product
      </a>
    </div>
  );
}
