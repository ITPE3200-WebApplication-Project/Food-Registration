import { useEffect, useState } from "react";
import type { IProduct } from "../../../types";
import DeleteModal from "../../shared/DeleteModal";
import { productApi } from "../../../services/api";
import { authService } from "../../../services/auth";
import { Navigate } from "react-router";

export default function MyProducts() {
  const { loggedIn } = authService.getUser();
  const [products, setProducts] = useState<IProduct[]>([]);
  useEffect(() => {
    productApi.getMyProducts().then(setProducts);
  }, []);

  const [showModal, setShowModal] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<{
    id: number;
    name: string;
  } | null>(null);

  if (!loggedIn) {
    return <Navigate to="/login" />;
  }

  const handleShowModal = (id: number, name: string) => {
    setSelectedProduct({ id, name });
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setSelectedProduct(null);
  };

  const handleConfirmDelete = async (id: number) => {
    //await onDelete(id);
    console.log(id);
    handleCloseModal();
  };

  return (
    <div className="container my-4">
      <h1 className="text-center mb-4">My Products</h1>
      <div
        className="card shadow-sm"
        style={{
          width: "100%",
          maxWidth: "none",
          height: "100%",
          maxHeight: "none",
        }}
      >
        <div className="card-body px-4 py-2">
          <div
            className="d-flex justify-content-between align-items-center"
            style={{ width: "100%", minHeight: "60px" }}
          >
            <a
              href="/products/create"
              className="btn btn-success rounded-pill px-3"
            >
              <i className="bi bi-plus-circle"></i> Add New Product
            </a>
          </div>
          <table className="table table-hover table-bordered mt-2">
            <thead className="table-primary">
              <tr>
                <th>Name</th>
                <th>Producer</th>
                <th>Nutrition Score</th>
                <td>Update</td>
                <td>Delete</td>
              </tr>
            </thead>
            <tbody>
              {products.map((product) => (
                <tr key={product.id}>
                  <td>{product.name}</td>
                  <td>{product.producer.name || "N/A"}</td>
                  <td className="text-center">
                    {product.nutritionScore ? (
                      <img
                        src={`/images/nutri-scores/Nutri-score-${product.nutritionScore}.svg.png`}
                        alt={`Nutrition Score ${product.nutritionScore}`}
                        className="nutrition-score-table"
                      />
                    ) : (
                      <span>N/A</span>
                    )}
                  </td>
                  <td className="text-center">
                    <a
                      href={`/products/update/${product.id}`}
                      className="btn btn-sm btn-warning rounded-pill px-3"
                    >
                      <i className="bi bi-pencil-square"></i> Update
                    </a>
                  </td>
                  <td className="text-center">
                    <button
                      className="btn btn-danger btn-sm rounded-pill px-3"
                      onClick={() => handleShowModal(product.id, product.name)}
                    >
                      <i className="bi bi-trash"></i> Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <DeleteModal
        show={showModal}
        productId={selectedProduct?.id || null}
        productName={selectedProduct?.name || ""}
        onClose={handleCloseModal}
        onConfirm={handleConfirmDelete}
      />
    </div>
  );
}
