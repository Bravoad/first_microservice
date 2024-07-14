# schemas.py
from pydantic import BaseModel, EmailStr

class UserBase(BaseModel):
    name: str
    email: str
    age: int


class UserCreate(UserBase):
    name: str
    email: str
    age: int


class User(UserBase):
    id: int

    class Config:
        orm_mode = True
