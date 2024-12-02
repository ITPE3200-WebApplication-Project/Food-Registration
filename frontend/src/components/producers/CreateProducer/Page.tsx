import { useState } from "react";
import { useNavigate } from "react-router";
import { producerApi } from "../../../services/api";
import { getBase64 } from "../../../services/file";
import type { ICreateProducerDTO } from "../../../types/dtos";
import { AxiosError } from "axios";
import { ProducerBasicInfo } from "./ProducerBasicInfo";
import { ImageUpload } from "./ImageUpload";

export default function CreateProducerPage() {
  const navigate = useNavigate();
  const [producer, setProducer] = useState<{
    name: string;
    description: string;
    imageFile: File | null;
  }>({
    name: "",
    description: "",
    imageFile: null,
  });

  const handleProducerChange = (updates: Partial<typeof producer>) => {
    setProducer((prev) => ({ ...prev, ...updates }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!producer.name) {
      alert("Please fill in all required fields. Name is required.");
      return;
    }
    if (!producer.description) {
      alert("Please fill in all required fields. Description is required.");
      return;
    }
    if (!producer.imageFile) {
      alert("Please fill in all required fields. Image is required.");
      return;
    }

    let createProducerDTO: Partial<ICreateProducerDTO> = {
      name: producer.name,
      description: producer.description,
    };

    await getBase64(producer.imageFile, (result) => {
      const extension =
        "." +
          producer.imageFile?.name.split(".")[
            producer.imageFile?.name.split(".").length - 1
          ] || "";

      createProducerDTO = {
        ...createProducerDTO,
        imageBase64: result.split(",")[1],
        imageFileExtension: extension,
      };
    });

    try {
      await producerApi.create(createProducerDTO as ICreateProducerDTO);
      navigate({
        pathname: "/producers",
        search: "?message=Producer created successfully&messageType=success",
      });
    } catch (error) {
      console.error("Failed to create producer:", error);
      if (error instanceof AxiosError) {
        navigate({
          search: `?message=${error.response?.data}&messageType=danger`,
        });
      } else {
        navigate({
          search: "?message=Failed to create producer&messageType=danger",
        });
      }
    }
  };

  return (
    <div className="container my-4">
      <h2 className="text-center mb-4">Create New Producer</h2>

      <div className="row justify-content-center">
        <div className="col-md-8">
          <form onSubmit={handleSubmit}>
            <ProducerBasicInfo
              producer={producer}
              onProducerChange={handleProducerChange}
            />

            <ImageUpload onProducerChange={handleProducerChange} />

            <div className="d-flex justify-content-start gap-2 mt-4">
              <button type="submit" className="btn btn-success">
                Create Producer
              </button>
              <a href="/producers" className="btn btn-secondary">
                Cancel
              </a>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
